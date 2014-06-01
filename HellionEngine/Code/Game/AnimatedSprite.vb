Namespace Game
    Public Class AnimatedSprite
        Inherits HellionEngine.Game.GameObject

        Public FrameIteration As Single = 1 / 15

        Private InternalSprite As SlimDX.Direct3D9.Sprite
        Private InternalTexture As HellionEngine.Rendering.Texture
        Private InternalDynamicLoad As Boolean

        Private AnimationFrames() As HellionEngine.Rendering.Texture
        Private AnimationFramePaths() As String

        Private InternalLooping As Boolean
        Private InternalFramesPerSecond As Integer
        Private InternalAnimationEnded As Boolean = False
        Private InternalCurrentFrame As Integer = 0
        Private CurrentTimeMS As Single = 0

        Public Sub New(AnimationName As String, Optional DynamicLoad As Boolean = False)
            MyBase.New()

            Me.InternalSprite = New SlimDX.Direct3D9.Sprite(HellionEngine.Rendering.DeviceInstance)
            Me.InternalDynamicLoad = DynamicLoad

            Dim directoryPath As String = "Animations/" + AnimationName + "/"
            Dim directory As New System.IO.DirectoryInfo(directoryPath)
            Dim files As System.IO.FileInfo() = directory.GetFiles()

            ReDim Me.AnimationFrames(files.Length - 1)
            If (DynamicLoad) Then
                ReDim Me.AnimationFramePaths(files.Length - 1)
            End If
            Dim TemporaryArray(files.Length - 1) As HellionEngine.Rendering.Texture

            Dim CurrentFileIndex As Integer = 0
            Dim FileType As String = "jpg"
            For Each File In files
                FileType = File.Extension
                REM TODO: Make DynamicLoad Work

                If (Not DynamicLoad) Then
                    TemporaryArray(CurrentFileIndex) = New HellionEngine.Rendering.Texture(directoryPath + File.Name)
                End If
                CurrentFileIndex += 1
            Next File

            REM Guarantee file ordering from 0 to count
            For Iteration As Integer = 0 To TemporaryArray.Length - 1
                Dim CurrentFileName As String = "Animations/" + AnimationName + "/frame" + Iteration.ToString() + FileType

                REM Look for Current Index
                For CurrentIndex As Integer = 0 To TemporaryArray.Length - 1
                    If (Not DynamicLoad) Then
                        Dim CurrentTexture As HellionEngine.Rendering.Texture = TemporaryArray(CurrentIndex)
                        If (CurrentTexture.Filepath = CurrentFileName) Then
                            Me.AnimationFrames(Iteration) = CurrentTexture
                            Exit For
                        End If
                    Else
                        Me.AnimationFramePaths(Iteration) = CurrentFileName
                    End If
                Next
            Next

            REM The first frame of our animation should exist when this sub ends, so if we're dyn loading everything, this has to be
            REM Done.
            If (DynamicLoad) Then
                Me.AnimationFrames(0) = New HellionEngine.Rendering.Texture(Me.AnimationFramePaths(0))
            End If
        End Sub

        Public Overrides Sub OnDraw()
            If (Not Me.Visible) Then
                Return
            End If

            Me.InternalSprite.Begin(SlimDX.Direct3D9.SpriteFlags.AlphaBlend Or SlimDX.Direct3D9.SpriteFlags.ObjectSpace)

            HellionEngine.Rendering.DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.World, SlimDX.Matrix.Identity)
            HellionEngine.Rendering.DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.World, SlimDX.Matrix.Transformation2D(New SlimDX.Vector2(0, 0), 0, Me.Scale.SlimDX, Me.RotationCenter.SlimDX, Me.Rotation.Radians, Me.Position.SlimDX))

            If (Me.AnimationFrames(Me.CurrentFrame) Is Nothing And Me.InternalDynamicLoad) Then
                Me.AnimationFrames(Me.CurrentFrame) = New HellionEngine.Rendering.Texture(Me.AnimationFramePaths(Me.CurrentFrame))
            End If

            Me.InternalSprite.Draw(Me.AnimationFrames(Me.CurrentFrame).RenderTexture, Me.Color.DirectXColor)
            Me.InternalSprite.End()
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.InternalSprite.Dispose()
        End Sub

        Public Property CurrentFrame() As Integer
            Get
                Return Me.InternalCurrentFrame
            End Get
            Set(value As Integer)
                REM Handle Weird Cases
                If (value > Me.AnimationFrames.Length() - 1) Then
                    value -= Me.AnimationFrames.Length() - 1
                ElseIf (value < 0) Then
                    value = (Me.AnimationFrames.Length() - 1) - Math.Abs(value)
                End If

                If (value = Me.AnimationFrames.Length() - 1 And Not Me.InternalAnimationEnded) Then
                    If (Not Me.Looping) Then
                        Me.InternalAnimationEnded = True
                    End If

                    RaiseEvent OnAnimationEnd()
                End If

                If (value < Me.AnimationFrames.Length() - 1) Then
                    Me.InternalAnimationEnded = False
                End If

                Me.InternalCurrentFrame = value
            End Set
        End Property

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            Me.CurrentTimeMS += DeltaTimeSeconds

            If (Me.CurrentTimeMS >= Me.FrameIteration) Then
                Me.CurrentTimeMS = 0

                REM Loop
                If (Me.CurrentFrame = Me.AnimationFrames.Length() - 1 And Me.Looping) Then
                    Me.CurrentFrame = 0
                ElseIf (Me.CurrentFrame < Me.AnimationFrames.Length() - 1) Then
                    Me.CurrentFrame += 1
                End If
            End If
        End Sub

        Public Overrides Property Dimensions() As HellionEngine.Support.Vector
            Get
                Return New HellionEngine.Support.Vector(Me.Scale.X * Me.AnimationFrames(0).Dimensions.X, Me.Scale.Y * Me.AnimationFrames(0).Dimensions.Y)
            End Get
            Set(value As HellionEngine.Support.Vector)
                Me.Scale = New HellionEngine.Support.Vector(value.X / Me.AnimationFrames(0).Dimensions.X, value.Y / Me.AnimationFrames(0).Dimensions.Y)
            End Set
        End Property

        Public Property FramesPerSecond() As Integer
            Get
                Return 15
            End Get
            Set(value As Integer)
                Me.FrameIteration = 1 / value
            End Set
        End Property

        Public Property Looping() As Boolean
            Get
                Return Me.InternalLooping
            End Get
            Set(value As Boolean)
                Me.InternalLooping = value

                If (value = True) Then
                    Me.InternalAnimationEnded = False
                End If
            End Set
        End Property


        Public Event OnAnimationEnd()
    End Class
End Namespace