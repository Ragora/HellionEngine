REM *********************************************************************
REM Rendering.vb
REM Module code that provides a working rendering subsystem.
REM
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Public Module Rendering
    Private SingletonInstance As ClassInstance
    Public DeviceInstance As SlimDX.Direct3D9.Device

    Public CameraPosition As HellionEngine.Support.Vector = New HellionEngine.Support.Vector(0, 0)
    Public CameraScale As HellionEngine.Support.Vector = New HellionEngine.Support.Vector(1, 1)
    Public CameraRotationCenter As HellionEngine.Support.Vector = New HellionEngine.Support.Vector(0, 0)
    Public CameraRotation As HellionEngine.Support.Rotation = New HellionEngine.Support.Rotation

    Public ReadOnly Property Singleton() As ClassInstance
        Get
            Return SingletonInstance
        End Get
    End Property

    Public Function Instantiate(MaximumRenderLayers As Integer) As ClassInstance
        If (SingletonInstance Is Nothing) Then
            SingletonInstance = New ClassInstance(MaximumRenderLayers)
            DeviceInstance = DeviceInstance
        End If

        Return SingletonInstance
    End Function

    Public Class ClassInstance
        Inherits HellionEngine.Core.EngineObject

        Public LayeringSystem As RenderLayerSystem

        Public LoadedTextures As HellionEngine.Support.DynamicArray

        Private InternalDisposed As Boolean

        Public Sub New(MaximumRenderLayers As Integer)
            Me.InternalDisposed = False
            Me.LoadedTextures = New HellionEngine.Support.DynamicArray(0)

            HellionEngine.Logging.Write("Rendering.vb [ClassInstance+New]: Initialization successful.")
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            If (Not (Me.LayeringSystem Is Nothing)) Then
                Me.LayeringSystem.Dispose()
            End If

            Me.LoadedTextures.IterationBegin(False)
            While (Not Me.LoadedTextures.IterationEnd())
                Me.LoadedTextures.IterationNext().Dispose()
            End While

            HellionEngine.Logging.Write("Rendering.vb [ClassInstance+Dispose]: Successfully deinitialized.")
        End Sub

        Public Overrides Sub OnDraw()
            MyBase.OnDraw()

            If (Not (Me.LayeringSystem Is Nothing)) Then
                Me.LayeringSystem.OnDraw()
            End If
        End Sub

        Public Function GetTexture(Filepath As String) As Texture
            Me.LoadedTextures.IterationBegin(False)
            While (Not Me.LoadedTextures.IterationEnd())
                Dim CurrentTexture As Texture = Me.LoadedTextures.IterationNext()

                If (CurrentTexture.Filepath = Filepath) Then
                    Return CurrentTexture
                End If
            End While

            Dim CreatedTexture As New Texture(Filepath)
            REM FIXME: Potential for infinite loop if Textures/Error.png does not exist.
            If (Not CreatedTexture.IsGood) Then
                Return Me.GetTexture("Textures/Error.png")
            End If

            Me.LoadedTextures.Append(CreatedTexture)
            Return CreatedTexture
        End Function
    End Class

    Public NotInheritable Class Texture
        Inherits HellionEngine.Core.EngineObject

        Public RenderTexture As SlimDX.Direct3D9.Texture

        Private InternalFilepath As String
        Private InternalIsGood As Boolean
        Private InternalDimensions As HellionEngine.Support.Vector

        Public Sub New(Filepath As String)
            MyBase.New()

            Me.InternalIsGood = False

            Try
                Me.RenderTexture = SlimDX.Direct3D9.Texture.FromFile(DeviceInstance, Filepath)
            Catch ex As SlimDX.Direct3D9.Direct3D9Exception
                HellionEngine.Logging.Write("Rendering.vb [Texture+New]: Failed to load " + Filepath)
                Me.Dispose()
                Return
            End Try

            Dim SurfaceDescription As SlimDX.Direct3D9.SurfaceDescription = Me.RenderTexture.GetLevelDescription(Me.RenderTexture.LevelOfDetail)
            Me.InternalDimensions = New HellionEngine.Support.Vector(SurfaceDescription.Width, SurfaceDescription.Height)

            HellionEngine.Logging.Write("Rendering.vb [Texture+New]: Loaded '" + Filepath + "' (" + Me.InternalDimensions.X.ToString() + "x" + Me.InternalDimensions.Y.ToString() + ") from local file repository.")

            Me.InternalIsGood = True
            Me.InternalFilepath = Filepath
        End Sub

        Public Shadows ReadOnly Property Dimensions() As HellionEngine.Support.Vector
            Get
                Return New HellionEngine.Support.Vector(Me.InternalDimensions.X, Me.InternalDimensions.Y)
            End Get
        End Property

        Public ReadOnly Property IsGood() As Boolean
            Get
                Return Me.InternalIsGood
            End Get
        End Property

        Public ReadOnly Property Filepath() As String
            Get
                Return New String(Me.InternalFilepath)
            End Get
        End Property

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.InternalIsGood = False
            Me.RenderTexture.Dispose()

            REM Remove us from the texture list
            REM This is really just a quick hack work around ingame textures becoming unloaded after exiting game
            SingletonInstance.LoadedTextures.RemoveObject(Me)
        End Sub
    End Class

    Public NotInheritable Class RenderLayerSystem
        Inherits HellionEngine.Core.EngineObject

        Public HasWorldRender As Boolean = False

        Private InternalLayerSystem As HellionEngine.Support.DynamicArray
        Private LayerCount As Integer

        Public Sub New(LayerCount As Integer)
            MyBase.New()

            Me.LayerCount = LayerCount

            Me.Empty()
        End Sub

        Public Sub Empty()
            If (Not (Me.InternalLayerSystem Is Nothing)) Then
                Me.InternalLayerSystem.Dispose()
            End If

            Me.InternalLayerSystem = New HellionEngine.Support.DynamicArray(0)
            For CurrentIteration As Integer = 0 To Me.LayerCount - 1
                Me.InternalLayerSystem.Append(New HellionEngine.Support.DynamicArray(0))
            Next
        End Sub

        Public Sub RemoveObject(Target As Object)
            For CurrentIteration As Integer = 0 To Me.LayerCount - 1
                Me.InternalLayerSystem.GetIndex(CurrentIteration).RemoveObject(Target)
            Next
        End Sub

        Public Overrides Sub OnDraw()
            MyBase.OnDraw()

            REM Guarantee our view matrix isn't modified
            DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.View, SlimDX.Matrix.Identity)
            DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.World, SlimDX.Matrix.Identity)

            Dim CurrentLayerID As Integer = 0
            Dim HitWorldLayer As Boolean = False
            Me.InternalLayerSystem.IterationBegin(False)
            While (Not Me.InternalLayerSystem.IterationEnd())
                If (Me.HasWorldRender = True And CurrentLayerID > (Me.InternalLayerSystem.Length / 2) - 1 And HitWorldLayer = False) Then
                    HitWorldLayer = True

                    REM Set camera information
                    DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.View, SlimDX.Matrix.Transformation2D(New SlimDX.Vector2(0, 0), 0, CameraScale.SlimDX, CameraRotationCenter.SlimDX, CameraRotation.Radians, CameraPosition.SlimDX))
                End If

                Dim CurrentArray As HellionEngine.Support.DynamicArray = Me.InternalLayerSystem.IterationNext()

                CurrentArray.IterationBegin(False)
                While (Not CurrentArray.IterationEnd())
                    CurrentArray.IterationNext().OnDraw()
                End While

                CurrentLayerID += 1
            End While
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()
        End Sub

        Public Sub Append(RenderLayer As Integer, NewObject As RenderedObject)
            Me.InternalLayerSystem.GetIndex(RenderLayer).Append(NewObject)
        End Sub
    End Class

    Public Interface RenderedObject
        Inherits HellionEngine.Core.DisposableObject

        Sub OnDraw()
        Property Position As HellionEngine.Support.Vector
        Property Scale As HellionEngine.Support.Vector
        Property Rotation As HellionEngine.Support.Rotation
        Property Color As HellionEngine.Support.Color
        Property RotationCenter As HellionEngine.Support.Vector
        Property Visible As Boolean

        Property RenderSystem As HellionEngine.RenderLayerSystem
    End Interface
End Module
