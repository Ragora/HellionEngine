Namespace Tanks
    Namespace BDog
        Public Class Grizzly
            Inherits HellionEngine.Game.HoverTank

            Private WeaponPrimed As Boolean = True
            Private WeaponFireTimer As HellionEngine.Support.Timing.TimedEvent

            Public Sub New()
                MyBase.New("Textures/BDog/Grizzly.png")

                Me.TurningSpeed = (Math.PI * 2) / 3
                Me.MaximumTurningSpeed = (Math.PI * 2) / 2

                Me.Scale = New HellionEngine.Support.Vector(0.55, 0.55)
                Me.RotationCenter = New HellionEngine.Support.Vector(Me.Dimensions.X / 2, Me.Dimensions.Y / 2)

                Me.Mass = 10
            End Sub

            Public Overrides Sub Dispose()
                MyBase.Dispose()

                PlayUI.RenderLayerSystem.RemoveObject(Me)
            End Sub

            Public Overrides Sub OnPhysicsUpdate(DeltaTimeSeconds As Single)
                If (Me.HitPoints <= 0 And Me.HitPoints <> -9999) Then
                    Me.HitPoints = -9999
                    Me.Position.X = 99999999
                    PlayUI.Game.OnTankDestroyed(Me)
                Else
                    MyBase.OnPhysicsUpdate(DeltaTimeSeconds)

                    If (Me.FiringWeapon And Me.WeaponPrimed) Then
                        Dim Result As New Battlezone.Weapons.SPStabber(Me.ForwardVector)
                        Result.Position = Me.Center + (Me.ForwardVector * 35)
                        PlayUI.RenderLayerSystem.Append(4, Result)
                        HellionEngine.Physics.Singleton.SimObjects.Append(Result)

                        Me.WeaponPrimed = False
                        Me.WeaponFireTimer = New HellionEngine.Support.Timing.TimedEvent(1000, False)
                        AddHandler Me.WeaponFireTimer.OnFire, AddressOf Me.WeaponFireTimer_Fire
                    End If
                End If
            End Sub

            Private Sub WeaponFireTimer_Fire()
                Me.WeaponPrimed = True
            End Sub
        End Class
    End Namespace
End Namespace