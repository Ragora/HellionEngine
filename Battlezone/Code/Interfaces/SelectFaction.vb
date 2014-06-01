Namespace Interfaces
    Public Class SelectFaction
        Inherits Battlezone.InterfaceSystem.InterfaceBase

        Private CCAAnimation As HellionEngine.Game.AnimatedSprite
        Private NSAAnimation As HellionEngine.Game.AnimatedSprite

        Public Sub New()
            MyBase.New("SelectFaction", 3)

            Dim Background As New HellionEngine.Game.Sprite("Textures/Interface/SelectFaction.png")
            Background.Dimensions = New HellionEngine.Support.Vector(1024, 768)
            Me.RenderLayerSystem.Append(1, Background)

            Dim CCAButton As New HellionEngine.Game.Button("Textures/Buttons/Russia_Up.png", "Textures/Buttons/Russia_Up.png", "Textures/Buttons/Russia_Moused.png")
            CCAButton.Position = New HellionEngine.Support.Vector(747, 95)
            CCAButton.Scale = New HellionEngine.Support.Vector(1.4, 1.21)
            AddHandler CCAButton.OnClicked, AddressOf Me.ButtonCCA_Clicked
            AddHandler CCAButton.MousedOver, AddressOf Me.ButtonCCA_MouseOver
            Me.RenderLayerSystem.Append(2, CCAButton)
            Me.UpdateObjectList.Append(CCAButton)

            Dim NSAButton As New HellionEngine.Game.Button("Textures/Buttons/America_Up.png", "Textures/Buttons/America_Up.png", "Textures/Buttons/America_Moused.png")
            NSAButton.Position = New HellionEngine.Support.Vector(103, 95)
            NSAButton.Scale = New HellionEngine.Support.Vector(1.4, 1.21)
            AddHandler NSAButton.OnClicked, AddressOf Me.ButtonNSA_Clicked
            AddHandler NSAButton.MousedOver, AddressOf Me.ButtonNSA_MouseOver
            Me.RenderLayerSystem.Append(2, NSAButton)
            Me.UpdateObjectList.Append(NSAButton)

            Dim BackButton As New HellionEngine.Game.Button("Textures/Buttons/Back_Up.png", "Textures/Buttons/Back_Up.png", "Textures/Buttons/Back_Moused.png")
            BackButton.Position = New HellionEngine.Support.Vector(60, 3)
            BackButton.Scale = New HellionEngine.Support.Vector(0.37, 0.34)
            AddHandler BackButton.OnClicked, AddressOf Me.BackButton_Clicked
            Me.RenderLayerSystem.Append(0, BackButton)
            Me.UpdateObjectList.Append(BackButton)

            Me.CCAAnimation = New HellionEngine.Game.AnimatedSprite("CCAPoster", True)
            Me.CCAAnimation.Position = New HellionEngine.Support.Vector(403, 60)
            Me.CCAAnimation.Scale = New HellionEngine.Support.Vector(0.87, 1.3)
            Me.CCAAnimation.Looping = False
            Me.CCAAnimation.Visible = False
            Me.RenderLayerSystem.Append(2, Me.CCAAnimation)
            Me.UpdateObjectList.Append(Me.CCAAnimation)

            Me.NSAAnimation = New HellionEngine.Game.AnimatedSprite("NSAPoster", True)
            Me.NSAAnimation.Position = New HellionEngine.Support.Vector(403, 60)
            Me.NSAAnimation.Scale = New HellionEngine.Support.Vector(0.87, 1.3)
            Me.NSAAnimation.Looping = False
            Me.NSAAnimation.Visible = False
            Me.RenderLayerSystem.Append(2, Me.NSAAnimation)
            Me.UpdateObjectList.Append(Me.NSAAnimation)
        End Sub

        Private Sub BackButton_Clicked()
            Battlezone.InterfaceSystem.ActiveInterfaceName = "Main"
        End Sub

        Private Sub ButtonCCA_MouseOver()
            If (Not Me.CCAAnimation.Visible) Then
                Me.CCAAnimation.CurrentFrame = 0
            End If

            Me.CCAAnimation.Visible = True
            Me.NSAAnimation.Visible = False
        End Sub

        Private Sub ButtonNSA_MouseOver()
            If (Not Me.NSAAnimation.Visible) Then
                Me.NSAAnimation.CurrentFrame = 0
            End If

            Me.CCAAnimation.Visible = False
            Me.NSAAnimation.Visible = True
        End Sub

        Private Sub ButtonCCA_Clicked()
            PlayUI.Game = New Battlezone.GameModes.BZGame(PlayUI, "Textures/Map/Moon.png", True)
            PlayUI.Game.InitializeGame()

            Battlezone.InterfaceSystem.ActiveInterfaceName = "Play"
        End Sub

        Private Sub ButtonNSA_Clicked()
            PlayUI.Game = New Battlezone.GameModes.BZGame(PlayUI, "Textures/Map/Moon.png", False)
            PlayUI.Game.InitializeGame()

            Battlezone.InterfaceSystem.ActiveInterfaceName = "Play"
        End Sub
    End Class
End Namespace