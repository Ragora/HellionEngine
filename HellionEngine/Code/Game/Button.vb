Namespace Game
    Public Class Button
        Inherits HellionEngine.Game.GameObject

        Private InternalSprite As SlimDX.Direct3D9.Sprite
        Private InternalUpTexture As HellionEngine.Rendering.Texture
        Private InternalDownTexture As HellionEngine.Rendering.Texture
        Private InternalMousedTexture As HellionEngine.Rendering.Texture

        Private IsMousedOver As Boolean = False
        Private IsPressed As Boolean = False
        Private IsClicked As Boolean = False

        Public Sub New(UpFilepath As String, DownFilePath As String, MousedFilePath As String)
            MyBase.New()

            Me.InternalSprite = New SlimDX.Direct3D9.Sprite(HellionEngine.Rendering.DeviceInstance)

            If (UpFilepath <> "") Then
                Me.InternalUpTexture = HellionEngine.Rendering.Singleton.GetTexture(UpFilepath)
            End If

            If (DownFilePath <> "") Then
                Me.InternalDownTexture = HellionEngine.Rendering.Singleton.GetTexture(DownFilePath)
            End If

            If (MousedFilePath <> "") Then
                Me.InternalMousedTexture = HellionEngine.Rendering.Singleton.GetTexture(MousedFilePath)
            End If
        End Sub

        Public Overrides Sub OnDraw()
            MyBase.OnDraw()

            Dim TextureToDraw As HellionEngine.Rendering.Texture = Nothing
            If (Me.IsMousedOver) Then
                TextureToDraw = Me.InternalMousedTexture
            End If

            REM Quick Hack to prevent rendering of a sprite that's not needed
            If (Not Me.IsMousedOver And Not Me.IsClicked) Then
                TextureToDraw = Me.InternalUpTexture
            End If

            If (Me.IsClicked) Then
                TextureToDraw = Me.InternalDownTexture
            End If

            Me.InternalSprite.Begin(SlimDX.Direct3D9.SpriteFlags.AlphaBlend Or SlimDX.Direct3D9.SpriteFlags.ObjectSpace)

            HellionEngine.Rendering.DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.World, SlimDX.Matrix.Identity)
            HellionEngine.Rendering.DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.World, SlimDX.Matrix.Transformation2D(New SlimDX.Vector2(0, 0), 0, Me.Scale.SlimDX, Me.RotationCenter.SlimDX, Me.Rotation.Radians, Me.Position.SlimDX))

            Me.InternalSprite.Draw(TextureToDraw.RenderTexture, Me.Color.DirectXColor)
            Me.InternalSprite.End()
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.InternalSprite.Dispose()
        End Sub


        Public Overrides Sub OnMousedOver(MousePosition As HellionEngine.Support.Vector)
            MyBase.OnMousedOver(MousePosition)

            Me.IsMousedOver = True
        End Sub

        Public Overrides Sub OnMousedAway(MousePosition As HellionEngine.Support.Vector)
            MyBase.OnMousedAway(MousePosition)

            Me.IsMousedOver = False
            Me.IsClicked = False
        End Sub

        Public Overrides Sub OnMouseButtonClicked(MouseButton As HellionEngine.Input.MouseButton, MousePosition As HellionEngine.Support.Vector)
            MyBase.OnMouseButtonClicked(MouseButton, MousePosition)

            Me.IsClicked = True
        End Sub

        Public Overrides Sub OnMouseButtonReleased(MouseButton As HellionEngine.Input.MouseButton, MousePosition As HellionEngine.Support.Vector)
            MyBase.OnMouseButtonReleased(MouseButton, MousePosition)

            If (Me.IsClicked And Me.IsMousedOver) Then
                Me.IsClicked = False
                Me.IsMousedOver = False

                RaiseEvent OnClicked()
            End If
        End Sub

        Public Overrides Property Dimensions() As HellionEngine.Support.Vector
            Get
                Return New HellionEngine.Support.Vector(Me.Scale.X * Me.InternalUpTexture.Dimensions.X, Me.Scale.Y * Me.InternalUpTexture.Dimensions.Y)
            End Get
            Set(value As HellionEngine.Support.Vector)
                Me.Scale = New HellionEngine.Support.Vector(value.X / Me.InternalUpTexture.Dimensions.X, value.Y / Me.InternalUpTexture.Dimensions.Y)
            End Set
        End Property

        Public Event OnClicked()
    End Class
End Namespace