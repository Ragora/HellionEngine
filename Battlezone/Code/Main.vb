Public Module Main
    Public InterfaceLogo As HellionEngine.Rendering.RenderLayerSystem

    Public LogoUI As Battlezone.Interfaces.Logo
    Public StartUI As Battlezone.Interfaces.Start
    Public MainUI As Battlezone.Interfaces.Main
    Public SelectFactionUI As Battlezone.Interfaces.SelectFaction
    Public PlayUI As Battlezone.Interfaces.Play
    Public EscapeUI As Battlezone.Interfaces.Escape
    Public DeathUI As Battlezone.Interfaces.Death

    Private Engine As HellionEngine.EngineInstance.InstanceClass

    Public Sub Main()
        REM Create the instance
        Engine = HellionEngine.EngineInstance.Instantiate()
        Engine.Title = "Battlezone"
        Engine.Width = 1024
        Engine.Height = 768
        Engine.FPSInTitle = True

        REM Bind handles
        AddHandler Engine.OnUpdated, AddressOf OnUpdateHandler
        AddHandler Engine.OnReady, AddressOf OnReadyHandler
        AddHandler Engine.OnDispose, AddressOf OnDisposeHandler

        REM Turn the key
        Engine.Start()
    End Sub

    Public Sub OnUpdateHandler(DeltaTimeSeconds)

    End Sub

    Public Sub OnReadyHandler()
        Randomize()

        REM Init UI's
        LogoUI = New Battlezone.Interfaces.Logo
        StartUI = New Battlezone.Interfaces.Start
        MainUI = New Battlezone.Interfaces.Main
        SelectFactionUI = New Battlezone.Interfaces.SelectFaction
        PlayUI = New Battlezone.Interfaces.Play
        EscapeUI = New Battlezone.Interfaces.Escape
        DeathUI = New Battlezone.Interfaces.Death

        REM Set Async Textures
        Engine.AddAsyncTexture("Textures/CCA/Czar.png")
        Engine.AddAsyncTexture("Textures/CCA/Flanker.png")
        Engine.AddAsyncTexture("Textures/BDog/Grizzly.png")
        Engine.AddAsyncTexture("Textures/Bdog/Razor.png")

        Battlezone.InterfaceSystem.ActiveInterfaceName = "Logo"
    End Sub

    Public Sub OnDisposeHandler()
        LogoUI.Dispose()
        StartUI.Dispose()
        MainUI.Dispose()
        SelectFactionUI.Dispose()
        PlayUI.Dispose()
        EscapeUI.Dispose()
    End Sub
End Module
