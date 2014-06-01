Public Module GameModes
    Public Class BZGame
        Inherits HellionEngine.Core.EngineObject

        Private PlayUI As Battlezone.Interfaces.Play = Nothing

        Private TopLeftTerrain As HellionEngine.Game.Sprite = Nothing
        Private TopRightTerrain As HellionEngine.Game.Sprite = Nothing
        Private BottomLeftTerrain As HellionEngine.Game.Sprite = Nothing
        Private BottomRightTerrain As HellionEngine.Game.Sprite = Nothing

        Private InvertedTurn As Boolean = False
        Private InvertedStrafe As Boolean = False

        Private PlayerTank As HellionEngine.Game.HoverTank = Nothing
        Private PlayerSelectedTank As Boolean = False
        Private CurrentSelectedTank As Integer = 0 REM Start off with Czar/Grizzly

        Private ActiveAICount As Integer = 0

        Public PlayerSelectedCCA As Boolean = False

        Public Sub New(PlayUI As Battlezone.Interfaces.Play, GroundTexture As String, SelectedCCA As Boolean)
            MyBase.New()

            REM Create the physics sim instance if it isn't already running
            If (HellionEngine.Physics.Singleton Is Nothing) Then
                HellionEngine.Physics.Instantiate()
            End If

            Me.PlayerSelectedCCA = SelectedCCA

            Me.PlayUI = PlayUI

            Me.TopLeftTerrain = New HellionEngine.Game.Sprite(GroundTexture)
            Me.PlayUI.RenderLayerSystem.Append(3, Me.TopLeftTerrain)

            Me.TopRightTerrain = New HellionEngine.Game.Sprite(GroundTexture)
            Me.PlayUI.RenderLayerSystem.Append(3, Me.TopRightTerrain)

            Me.BottomLeftTerrain = New HellionEngine.Game.Sprite(GroundTexture)
            Me.PlayUI.RenderLayerSystem.Append(3, Me.BottomLeftTerrain)

            Me.BottomRightTerrain = New HellionEngine.Game.Sprite(GroundTexture)
            Me.PlayUI.RenderLayerSystem.Append(3, Me.BottomRightTerrain)

            REM Stick our terrains at the proper spots
            Me.BottomLeftTerrain.Position -= New HellionEngine.Support.Vector(Me.TopLeftTerrain.Dimensions.X, 0)
            Me.TopLeftTerrain.Position -= New HellionEngine.Support.Vector(Me.TopLeftTerrain.Dimensions.X, Me.TopLeftTerrain.Dimensions.Y)
            Me.TopRightTerrain.Position -= New HellionEngine.Support.Vector(0, Me.TopLeftTerrain.Dimensions.Y)
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            HellionEngine.Physics.Singleton.Dispose()
        End Sub

        Public Overridable Sub OnTankDestroyed(Tank As HellionEngine.Game.HoverTank)
            Tank.Objective = Nothing

            If (Tank Is Me.PlayerTank) Then
                Me.PlayerTank = Nothing

                Battlezone.InterfaceSystem.ActiveInterfaceName = "Death"
            Else
                Me.ActiveAICount -= 1
            End If
        End Sub

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            MyBase.OnUpdate(DeltaTimeSeconds)

            If (Not (Me.PlayerTank Is Nothing)) Then
                Dim WindowDimensions As New HellionEngine.Support.Vector(HellionEngine.EngineInstance.Singleton.Width, HellionEngine.EngineInstance.Singleton.Height)
                HellionEngine.Rendering.CameraPosition = -Me.PlayerTank.Center + (WindowDimensions / 2)
            End If

            REM Place Tanks if we need to (there should be 4 at a time
            If (Me.ActiveAICount < 3 And Me.PlayerSelectedTank) Then
                Dim ResultTank As HellionEngine.Game.HoverTank = Nothing

                REM CCA?
                If (Me.PlayerSelectedCCA) Then
                    REM Spawn Grizzly
                    If (HellionEngine.Support.Random.RandomInteger(0, 10) < 5) Then
                        ResultTank = New Battlezone.Tanks.BDog.Grizzly()
                    Else
                        ResultTank = New Battlezone.Tanks.BDog.Razor()
                    End If
                Else
                    If (HellionEngine.Support.Random.RandomInteger(0, 10) < 5) Then
                        ResultTank = New Battlezone.Tanks.CCA.Czar()
                    Else
                        ResultTank = New Battlezone.Tanks.CCA.Flanker()
                    End If
                End If

                ResultTank.Position = HellionEngine.Support.Random.RandomVectorInRadius(Me.PlayerTank.Position, 300, 1000)
                ResultTank.Objective = New Battlezone.Tanks.AI.AIObjectiveEngageObject(ResultTank, Me.PlayerTank)
                Me.PlayUI.RenderLayerSystem.Append(4, ResultTank)

                Me.ActiveAICount += 1
            End If
        End Sub

        Public Overridable Sub OnPlayerReady()

        End Sub

        Public Overridable Sub InitializeGame()
            Dim MusicToPlay As String = "Music/AMA"
            MusicToPlay += HellionEngine.Support.Random.RandomInteger(1, 3).ToString() + ".wav"
            HellionEngine.Sound.Singleton.PlayMusic(MusicToPlay, True)

            REM Create Our Starter Tank, since we need to select our initial one from here
            If (Me.PlayerSelectedCCA) Then
                Me.PlayerTank = New Battlezone.Tanks.CCA.Czar()
            Else
                Me.PlayerTank = New Battlezone.Tanks.BDog.Grizzly()
            End If

            Me.PlayerTank.Color.Alpha = 170
            Me.PlayUI.RenderLayerSystem.Append(4, PlayerTank)
        End Sub

        Public Overrides Sub OnKeyPressed(Key As HellionEngine.Key)
            MyBase.OnKeyPressed(Key)

            If (Not (Me.PlayerTank Is Nothing)) Then
                Select Case Key
                    Case Battlezone.Keymap.FireWeapon
                        If (Not Me.PlayerSelectedTank) Then
                            Me.PlayerSelectedTank = True
                            Me.PlayerTank.Color.Alpha = 255
                            Me.OnPlayerReady()
                        Else
                            Me.PlayerTank.FiringWeapon = True
                        End If

                        REM Forward and Back Movement
                    Case Battlezone.Keymap.MoveForward
                        Me.PlayerTank.ForwardMoving = True
                    Case Battlezone.Keymap.MoveBackward
                        Me.PlayerTank.BackwardMoving = True

                        REM Left and Right Movement
                    Case Battlezone.Keymap.MoveLeft
                        REM If (Me.ActivePlayer.RotationRadians >= -1.5 And Me.ActivePlayer.RotationRadians <= 1.5) Then
                        Me.PlayerTank.LeftwardMoving = True
                        REM Else
                        REM Me.ActivePlayer.RightwardMoving = True
                        REM Me.InvertedStrafe = True
                        REM End If
                    Case Battlezone.Keymap.MoveRight
                        REM If (Me.ActivePlayer.RotationRadians >= -1.5 And Me.ActivePlayer.RotationRadians <= 1.5) Then
                        Me.PlayerTank.RightwardMoving = True
                        REM  Else
                        REM  Me.InvertedStrafe = True
                        REM Me.ActivePlayer.LeftwardMoving = True
                        REM  End If
                    Case Battlezone.Keymap.TurnLeft
                        If (Me.PlayerTank.Rotation.Radians >= 0 And Me.PlayerTank.Rotation.Radians <= Math.PI) Then
                            Me.PlayerTank.LeftTurning = True
                        Else
                            Me.InvertedTurn = True
                            Me.PlayerTank.RightTurning = True
                        End If
                    Case Battlezone.Keymap.TurnRight
                        If (Me.PlayerTank.Rotation.Radians <= Math.PI And Me.PlayerTank.Rotation.Radians >= 0) Then
                            Me.PlayerTank.RightTurning = True
                        Else
                            Me.InvertedTurn = True
                            Me.PlayerTank.LeftTurning = True
                        End If

                        REM Braking
                    Case Battlezone.Keymap.Brake
                        Me.PlayerTank.Braking = True

                        REM Switch Weapons
                    Case Battlezone.Keymap.SwitchWeapon
                        If (Not Me.PlayerSelectedTank) Then
                            ' Me.PlayerTank.Dispose()
                            REM To hell with .NET ...
                            PlayUI.RenderLayerSystem.RemoveObject(Me.PlayerTank)
                            HellionEngine.Physics.Singleton.SimObjects.RemoveObject(Me.PlayerTank)

                            Dim NewTank As HellionEngine.Game.HoverTank

                            If (Me.PlayerSelectedCCA) Then
                                If (Me.CurrentSelectedTank = 0) Then
                                    NewTank = New Battlezone.Tanks.CCA.Flanker()
                                    Me.CurrentSelectedTank = 1
                                Else
                                    NewTank = New Battlezone.Tanks.CCA.Czar()
                                    Me.CurrentSelectedTank = 0
                                End If
                            Else
                                If (Me.CurrentSelectedTank = 0) Then
                                    NewTank = New Battlezone.Tanks.BDog.Razor()
                                    Me.CurrentSelectedTank = 1
                                Else
                                    NewTank = New Battlezone.Tanks.BDog.Grizzly()
                                    Me.CurrentSelectedTank = 0
                                End If
                            End If

                            NewTank.Position = New HellionEngine.Support.Vector(Me.PlayerTank.Position.X, Me.PlayerTank.Position.Y)
                            NewTank.Rotation.Radians = Me.PlayerTank.Rotation.Radians
                            NewTank.Velocity = New HellionEngine.Support.Vector(Me.PlayerTank.Velocity.X, Me.PlayerTank.Velocity.Y)

                            '    Me.PlayerTank.Dispose()
                            Me.PlayerTank = NewTank
                            Me.PlayerTank.Color.Alpha = 170
                            Me.PlayUI.RenderLayerSystem.Append(4, Me.PlayerTank)
                        End If
                End Select
            End If
        End Sub

        Public Overrides Sub OnKeyReleased(Key As HellionEngine.Key)
            MyBase.OnKeyReleased(Key)

            If (Not (Me.PlayerTank Is Nothing)) Then
                Select Case Key
                    Case Battlezone.Keymap.MoveForward
                        Me.PlayerTank.ForwardMoving = False
                    Case Battlezone.Keymap.MoveBackward
                        Me.PlayerTank.BackwardMoving = False

                    Case Battlezone.Keymap.MoveLeft
                        REM  If (Me.InvertedStrafe) Then
                        REM Me.ActivePlayer.RightwardMoving = False
                        REM  Me.InvertedStrafe = False
                        REM  Else
                        Me.PlayerTank.LeftwardMoving = False
                        REM  End If
                    Case Battlezone.Keymap.MoveRight
                        REM If (Me.InvertedStrafe) Then
                        REM Me.ActivePlayer.LeftwardMoving = False
                        REM Me.InvertedStrafe = False
                        REM Else
                        Me.PlayerTank.RightwardMoving = False
                        REM End If

                    Case Battlezone.Keymap.FireWeapon
                        Me.PlayerTank.FiringWeapon = False

                    Case Battlezone.Keymap.TurnLeft
                        If (Me.InvertedTurn) Then
                            Me.PlayerTank.RightTurning = False
                            Me.InvertedTurn = False
                        Else
                            Me.PlayerTank.LeftTurning = False
                        End If
                    Case Battlezone.Keymap.TurnRight
                        If (Me.InvertedTurn) Then
                            Me.PlayerTank.LeftTurning = False
                            Me.InvertedTurn = False
                        Else
                            Me.PlayerTank.RightTurning = False
                        End If
                    Case Battlezone.Keymap.Brake
                        Me.PlayerTank.Braking = False
                End Select
            End If
        End Sub
    End Class
End Module
