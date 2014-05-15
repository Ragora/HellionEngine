REM *********************************************************************
REM Color.vb
REM Custom color object class that is used to separate the main coding
REM from SlimDX's API. It also provides some functionality that was
REM not available in SlimDX directly without some hacking.
REM 
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Namespace Support
	Public Class Color
		Private InternalColor As SlimDX.Color4

		Public ReadOnly Property DirectXColor()
			Get
		    	Return Me.InternalColor
		    End Get
		End Property

		Public Property Red() As Integer
		    Get
		        Return Me.InternalColor.Red * 255
		    End Get
		    Set(value As Integer)
		    	If (value > 255) Then
		        	value -= 255
		        ElseIf (value < 0) Then
		            value = 255 - Math.Abs(value)
		        End If

		        Me.InternalColor.Red = value / 255
		    End Set
		End Property

		Public Property Green() As Integer
			Get
				Return Me.InternalColor.Green * 255
		    End Get
		    Set(value As Integer)
				If (value > 255) Then
		       		value -= 255
		        ElseIf (value < 0) Then
		            value = 255 - Math.Abs(value)
		        End If

		        Me.InternalColor.Green = value / 255
		  	End Set
		End Property

		Public Property Blue() As Integer
			Get
		    	Return Me.InternalColor.Blue * 255
		    End Get
		    Set(value As Integer)
		    	If (value > 255) Then
		        	value -= 255
		        ElseIf (value < 0) Then
		            value = 255 - Math.Abs(value)
		        End If

		        Me.InternalColor.Blue = value / 255
		 	End Set
		End Property


		Public Property Alpha() As Integer
			Get
				Return Me.InternalColor.Alpha * 255
		    End Get
		    Set(value As Integer)
		    	If (value > 255) Then
		        	value -= 255
		        ElseIf (value < 0) Then
		            value = 255 - Math.Abs(value)
		        End If

		            Me.InternalColor.Alpha = value / 255
		        End Set
	   End Property

		Public Sub New(NewRed As Integer, NewGreen As Integer, NewBlue As Integer, NewAlpha As Integer)
			Me.InternalColor = New SlimDX.Color4

		    Me.Red = NewRed
		    Me.Green = NewGreen
		    Me.Blue = NewBlue
		    Me.Alpha = NewAlpha
		End Sub
	End Class
End Namespace
