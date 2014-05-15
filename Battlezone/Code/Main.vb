Public Module Main
    Public InterfaceLogo As HellionEngine.Rendering.RenderLayerSystem

    Private LogoUI As Battlezone.Interfaces.Logo
    Private StartUI As Battlezone.Interfaces.Start
    Private MainUI As Battlezone.Interfaces.Main
    Private SelectFactionUI As Battlezone.Interfaces.SelectFaction

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
        REM Init UI's
        LogoUI = New Battlezone.Interfaces.Logo
        StartUI = New Battlezone.Interfaces.Start
        MainUI = New Battlezone.Interfaces.Main
        SelectFactionUI = New Battlezone.Interfaces.SelectFaction

        REM Set Async Textures
        Engine.AddAsyncAnimation("Battlezone")

        Battlezone.InterfaceSystem.ActiveInterfaceName = "Logo"
    End Sub

    Public Sub OnDisposeHandler()
        LogoUI.Dispose()
        StartUI.Dispose()
        MainUI.Dispose()
        SelectFactionUI.Dispose()
    End Sub
End Module
