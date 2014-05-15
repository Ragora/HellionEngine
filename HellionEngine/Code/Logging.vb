REM *********************************************************************
REM LoggingSystem.vb
REM Module Logging code that is used by the rest of the code in the
REM engine.
REM
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Public Module Logging
    REM Enumeration that is used to track importance of each log level.
    Enum LoggingLevel
        LevelInformation = 0
        LevelWarning = 1
        LevelError = 2
        LevelFatal = 3
    End Enum

    Private IsInitialized As Boolean = False
    Private CanWrite As Boolean = True

    REM The only way of actually interfacing with the logging code as long as the application is alive.
    Public Sub Write(Output As String)
        If (Not IsInitialized) Then
            REM Determine if we can actually Write to our Logfile
            Try
                Dim TestStream As New System.IO.StreamWriter("LOG.txt", False)
                TestStream.WriteLine("Logging.vb [Logging-Write]: Logging Code Started Successfully.")
                TestStream.Close()
            Catch ex As System.IO.IOException
                CanWrite = False
                System.Console.WriteLine("Logging.vb [Write]: " + ex.Message)
            End Try

            IsInitialized = True
        End If

        System.Console.WriteLine(Output)
        REM Append to the LOG if we can actually write to it...
        If (CanWrite) Then
            Dim TestStream As New System.IO.StreamWriter("LOG.txt", True)
            TestStream.WriteLine(Output)
            TestStream.Close()
        End If
    End Sub
End Module