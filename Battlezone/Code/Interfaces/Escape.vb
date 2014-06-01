Namespace Interfaces
    Public Class Escape
        Inherits Battlezone.InterfaceSystem.InterfaceBase

        Public Sub New()
            MyBase.New("Escape", 4)

            REM Display the Main Texture
            Dim BackGround As New HellionEngine.Game.Sprite("Textures/Interface/Escape.png")
            BackGround.Dimensions = New HellionEngine.Support.Vector(1024, 768)
            Me.RenderLayerSystem.Append(1, BackGround)

            REM Load the Back Button
            Dim BackButton As New HellionEngine.Game.Button("Textures/Buttons/Back_Up.png", "Textures/Buttons/Back_Up.png", "Textures/Buttons/Back_Moused.png")
            BackButton.Position = New HellionEngine.Support.Vector(360, 399)
            BackButton.Scale = New HellionEngine.Support.Vector(0.6, 0.8)
            AddHandler BackButton.OnClicked, AddressOf Me.ButtonBack_Clicked
            Me.RenderLayerSystem.Append(0, BackButton)
            Me.UpdateObjectList.Append(BackButton)

            REM Load the Exit Button
            Dim ExitButton As New HellionEngine.Game.Button("Textures/Buttons/Exit_Up.png", "Textures/Buttons/Exit_Up.png", "Textures/Buttons/Exit_Moused.png")
            ExitButton.Position = New HellionEngine.Support.Vector(360, 526)
            ExitButton.Scale = New HellionEngine.Support.Vector(0.62, 1.5)
            AddHandler ExitButton.OnClicked, AddressOf Me.ButtonExitGame_Clicked
            Me.RenderLayerSystem.Append(0, ExitButton)
            Me.UpdateObjectList.Append(ExitButton)
        End Sub

        Public Overrides Sub OnWake(OldInterfaceName As String)
            MyBase.OnWake(OldInterfaceName)

            Me.BindKeyboardEvents()

            HellionEngine.Rendering.CameraScale = New HellionEngine.Support.Vector(1, 1)
        End Sub

        Public Overrides Sub OnSleep(NewInterfaceName As String)
            MyBase.OnSleep(NewInterfaceName)

            Me.UnbindKeyboardEvents()
        End Sub

        Public Overrides Sub OnKeyPressed(Key As HellionEngine.Input.Key)
            If (Key = HellionEngine.Key.Escape) Then
                Me.ButtonBack_Clicked()
            End If
        End Sub

        Private Sub ButtonExitGame_Clicked()
            REM Try and fix a bug where old game stuff is lingering around
            PlayUI.RenderLayerSystem.Empty()
            PlayUI.Game.Dispose()
            PlayUI.Game = Nothing

            Battlezone.InterfaceSystem.ActiveInterfaceName = "Main"
        End Sub

        Private Sub ButtonBack_Clicked()
            Battlezone.InterfaceSystem.ActiveInterfaceName = "Play"
        End Sub
    End Class
End Namespace