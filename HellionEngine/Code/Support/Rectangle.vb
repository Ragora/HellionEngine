REM *********************************************************************
REM Rectangle.vb
REM A custom rectangle class to replace the inbuilt one which seemed to
REM utilize integers and thusly caused truncation of the various singles
REM used within the engine.
REM 
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Namespace Support
	Public Class Rectangle
		Public Position As HellionEngine.Support.Vector
        Public Height As Single
        Public Width As Single

        Public Sub New(X As Single, Y As Single, NewWidth As Single, NewHeight As Single)
            Me.Position = New HellionEngine.Support.Vector(X, Y)
			Me.Width = NewWidth
			Me.Height = NewHeight
		End Sub

		Public Function Contains(X As Single, Y As Single)
			If (X >= Me.Position.X And Y >= Me.Position.Y And X <= Me.Right And Y <= Me.Bottom) Then
				Return True
			End If

            Return False
        End Function

        Public Function Contains(Position As HellionEngine.Support.Vector) As Boolean
            Return Me.Contains(Position.X, Position.Y)
        End Function

        REM These Set logic are wrong
        Public Property Top() As Single
            Get
            	Return Me.Position.Y
            End Get
            Set(value As Single)
                Me.Position.Y = value
            End Set
        End Property

        Public Property Left() As Single
           Get
               	Return Me.Position.X
           End Get
           Set(value As Single)
                Me.Position.X = value
           End Set
		End Property

        Public Property Bottom() As Single
           Get
                Return Me.Position.Y + Me.Height
           End Get
           Set(value As Single)
                Me.Position.Y += value + Me.Height
           End Set
        End Property

        Public Property Right() As Single
           Get
               Return Me.Position.X + Me.Width
           End Get
                Set(value As Single)
                    Me.Position.X += value + Me.Width
                End Set
           End Property
        End Class
    End Namespace
