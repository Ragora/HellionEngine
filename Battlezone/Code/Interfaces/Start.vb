Namespace Interfaces
    Public Class Start
        Inherits Battlezone.InterfaceSystem.InterfaceBase

        Private BattleZoneImage As HellionEngine.Game.Sprite
        Private StartImage As HellionEngine.Game.Sprite
        Private BackgroundImage As HellionEngine.Game.Sprite

        Private StartImagePulseState As Boolean = False

        Public Sub New()
            MyBase.New("Start", 2)

            Me.BattleZoneImage = New HellionEngine.Game.Sprite("Textures/Interface/Battlezone.png")
            Me.BattleZoneImage.Position = New HellionEngine.Support.Vector(100, -300)
            Me.BattleZoneImage.Scale = New HellionEngine.Support.Vector(0.8, 0.8)
            Me.RenderLayerSystem.Append(1, Me.BattleZoneImage)
            Me.BattleZoneImage.Visible = True

            Me.BackgroundImage = New HellionEngine.Game.Sprite("Textures/Interface/Intro.png")
            Me.BackgroundImage.Dimensions = New HellionEngine.Support.Vector(1024, 768)
            Me.RenderLayerSystem.Append(0, Me.BackgroundImage)
            Me.BackgroundImage.Visible = True
            Me.BackgroundImage.Color.Alpha = 0

            Me.StartImage = New HellionEngine.Game.Sprite("Textures/Interface/Enter.png")
            Me.StartImage.Visible = False
            Me.StartImage.Position = New HellionEngine.Support.Vector(400, 650)
            Me.StartImage.Scale = New HellionEngine.Support.Vector(0.25, 0.25)
            Me.RenderLayerSystem.Append(1, Me.StartImage)
        End Sub

        Public Overrides Sub OnWake(OldInterfaceName As String)
            MyBase.OnWake(OldInterfaceName)

            HellionEngine.Sound.Singleton.PlayMusic("Music/Intro.wav", False)

            ' BattleZone.Game.Globals.Core.Application.PreloadResources()

            Me.StartImage.Color.Alpha = 0
        End Sub

        Public Overrides Sub OnSleep(NewInterfaceName As String)
            MyBase.OnSleep(NewInterfaceName)
        End Sub

        Public Overrides Sub OnKeyPressed(Key As HellionEngine.Input.Key)
            If (Key = HellionEngine.Input.Key.Enter And Me.StartImage.Visible) Then
                Battlezone.ActiveInterfaceName = "Main"
            End If
        End Sub

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            MyBase.OnUpdate(DeltaTimeSeconds)

            If (Me.BattleZoneImage.Position.Y < 400) Then
                Me.BattleZoneImage.Position += New HellionEngine.Support.Vector(0, 650 * DeltaTimeSeconds)
            Else
                Me.BattleZoneImage.Position = New HellionEngine.Support.Vector(100, 400)

                If (Me.BackgroundImage.Color.Alpha >= 240) Then
                    Me.BackgroundImage.Color.Alpha = 255
                    Me.StartImage.Visible = True
                Else
                    Me.BackgroundImage.Color.Alpha += 100 * DeltaTimeSeconds
                    Me.StartImage.Visible = False
                End If
            End If

            If (Not Me.StartImagePulseState) Then
                Me.StartImage.Color.Alpha -= 150 * DeltaTimeSeconds

                If (Me.StartImage.Color.Alpha <= 25) Then
                    Me.StartImagePulseState = True
                End If
            Else
                Me.StartImage.Color.Alpha += 150 * DeltaTimeSeconds

                If (Me.StartImage.Color.Alpha >= 240) Then
                    Me.StartImagePulseState = False
                End If
            End If
        End Sub
    End Class
End Namespace