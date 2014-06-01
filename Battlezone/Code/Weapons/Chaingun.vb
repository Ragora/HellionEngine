Namespace Weapons
    Public Class Chaingun
        Inherits HellionEngine.Game.TracerProjectile

        Private LifeTimeTimer As HellionEngine.Support.Timing.TimedEvent

        Public Sub New(Heading As HellionEngine.Support.Vector)
            MyBase.New(1, "Textures/Weapons/Chaingun.png", Heading)

            Me.Scale = New HellionEngine.Support.Vector(0.55, 0.55)
            Me.RotationCenter = New HellionEngine.Support.Vector(Me.Dimensions.X / 2, Me.Dimensions.Y / 2)

            Me.MuzzleVelocity = 100

            Me.LifeTimeTimer = New HellionEngine.Support.Timing.TimedEvent(2000, False)
            AddHandler Me.LifeTimeTimer.OnFire, AddressOf Me.LifeTimeFire

            Me.DamageValue = 10
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            PlayUI.RenderLayerSystem.RemoveObject(Me)

            RemoveHandler Me.LifeTimeTimer.OnFire, AddressOf Me.LifeTimeFire
        End Sub

        Public Overrides Sub OnObjectHit(Other As HellionEngine.Game.HoverTank)
            MyBase.OnObjectHit(Other)

            'PlayUI.RenderLayerSystem.RemoveObject(Me)
            'HellionEngine.Physics.Singleton.SimObjects.RemoveObject(Me)

            REM Cheap Hack, destroy the thing by putting it out of bounds and let the thing expire
            REM Since the above causes a NULL reference exception
            Me.Position.X = 9999999999
        End Sub

        Public Sub LifeTimeFire()
            PlayUI.RenderLayerSystem.RemoveObject(Me)
            HellionEngine.Physics.Singleton.SimObjects.RemoveObject(Me)

            Me.Dispose()
        End Sub
    End Class
End Namespace