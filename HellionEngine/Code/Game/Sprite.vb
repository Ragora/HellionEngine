Namespace Game
    Public Class Sprite
        Inherits HellionEngine.Game.GameObject

        Private InternalSprite As SlimDX.Direct3D9.Sprite
        Private InternalTexture As HellionEngine.Rendering.Texture

        Public Sub New(Filepath As String)
            MyBase.New()

            Me.InternalSprite = New SlimDX.Direct3D9.Sprite(HellionEngine.Rendering.DeviceInstance)
            Me.InternalTexture = HellionEngine.Rendering.Singleton.GetTexture(Filepath)
        End Sub

        Public Overrides Sub OnDraw()
            MyBase.OnDraw()

            If (Not Me.Visible Or Me.Disposed() Or Me.InternalSprite.Disposed()) Then
                Return
            End If

            Me.InternalSprite.Begin(SlimDX.Direct3D9.SpriteFlags.AlphaBlend Or SlimDX.Direct3D9.SpriteFlags.ObjectSpace)

            HellionEngine.Rendering.DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.World, SlimDX.Matrix.Identity)
            HellionEngine.Rendering.DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.World, SlimDX.Matrix.Transformation2D(New SlimDX.Vector2(0, 0), 0, Me.Scale.SlimDX, Me.RotationCenter.SlimDX, Me.Rotation.Radians, Me.Position.SlimDX))

            Me.InternalSprite.Draw(Me.InternalTexture.RenderTexture, Me.Color.DirectXColor)
            Me.InternalSprite.End()
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.InternalSprite.Dispose()
        End Sub

        Public Overrides Property Dimensions() As HellionEngine.Support.Vector
            Get
                Return New HellionEngine.Support.Vector(Me.Scale.X * Me.InternalTexture.Dimensions.X, Me.Scale.Y * Me.InternalTexture.Dimensions.Y)
            End Get
            Set(value As HellionEngine.Support.Vector)
                Me.Scale = New HellionEngine.Support.Vector(value.X / Me.InternalTexture.Dimensions.X, value.Y / Me.InternalTexture.Dimensions.Y)
            End Set
        End Property
    End Class
End Namespace