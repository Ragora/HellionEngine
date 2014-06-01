REM *********************************************************************
REM DynamicArray.vb
REM A class which is an easier to use version of the built-in VB.NET
REM array objects. The most prominent feature of this array is that it
REM can be enlarged without destroying its contents, unlike VB.NET's
REM built-in arrays.
REM
REM This software is licensed under the MIT license. Please refer to
REM LICENSE.txt for more information.
REM *********************************************************************

Namespace Support
    Public Class DynamicArray
        Private InternalArray(0) As Object
        Private CurrentIterationIndex As Integer = 0
        Private IsIterating As Boolean = False
        Private IsForwardIterating As Boolean = False

        REM FIXME: No initial size should be specified.
        Public Sub New(NewSize As Integer)

        End Sub

        REM Sets the object to the given Index. Raises System.IO.
        Public Sub SetIndex(Index As Integer, NewObject As Object)
            If (Index < 0 Or Index >= Me.InternalArray.Length) Then
                Return
            End If

            Me.InternalArray(Index) = NewObject
        End Sub

        REM Returns the object at the given Index. Returns Nothing is Index is out of range.
        Public Function GetIndex(Index As Integer) As Object
            If (Index < 0 Or Index >= Me.InternalArray.Length) Then
                Return Nothing
            End If

            Return Me.InternalArray(Index)
        End Function

        REM Manages the Size of the array
        Public Property Length() As Integer
            Get
                Return Me.InternalArray.Length - 1 REM Offset for that last element that's always there for quick assignment
            End Get
            Set(value As Integer)
                Me.SetLength(value)
            End Set
        End Property

        Public ReadOnly Property LastElementID() As Integer
            Get
                Return Me.Length - 1
            End Get
        End Property

        Public Sub IterationBegin(Optional Backwards As Boolean = False)
            If (Not Backwards) Then
                Me.CurrentIterationIndex = 0
            Else
                Me.CurrentIterationIndex = Me.InternalArray.Length - 2
            End If

            If (Me.InternalArray.Length = 0) Then
                Me.IsIterating = False
                Return
            End If

            Me.IsIterating = True
            Me.IsForwardIterating = Not Backwards
        End Sub

        Public Function IterationEnd() As Boolean
            If (Me.CurrentIterationIndex >= Me.InternalArray.Length - 1 And Me.IsForwardIterating) Then
                Me.IsIterating = False
                Return True
            End If

            If (Me.CurrentIterationIndex <= -1 And Not Me.IsForwardIterating) Then
                Me.IsIterating = False
                Return True
            End If

            Return Not Me.IsIterating
        End Function

        Public Function IterationNext() As Object
            If (Not Me.IsIterating Or Not Me.IsForwardIterating) Then
                Return Nothing
            End If

            If (Me.CurrentIterationIndex >= Me.InternalArray.Length - 1) Then
                Me.IsIterating = False
            End If

            Dim Result As Object = Me.InternalArray(Me.CurrentIterationIndex)
            Me.CurrentIterationIndex += 1

            Return Result
        End Function

        Public Function IterationPrevious() As Object
            If (Not Me.IsIterating Or Me.IsForwardIterating) Then
                Return Nothing
            End If

            Dim Result As Object = Me.InternalArray(Me.CurrentIterationIndex)
            Me.CurrentIterationIndex -= 1
            If (Me.CurrentIterationIndex <= -1) Then
                Me.IsIterating = False
            End If

            Return Result
        End Function

        Public Sub SetLength(NewLength As Integer)
            Dim NewArray(NewLength) As Object

            Dim TotalCopiedElements As Integer = Me.InternalArray.Length

            If (TotalCopiedElements >= NewLength) Then
                TotalCopiedElements = NewLength
            End If

            REM Copy the Old Contents Over
            For CurrentIndex As Integer = 0 To TotalCopiedElements - 1
                NewArray(CurrentIndex) = Me.InternalArray(CurrentIndex)
            Next

            Me.InternalArray = NewArray
        End Sub

        Public Sub RemoveObject(TargetObject As Object)
            REM Locate the Object
            Dim TargetObjectIndex As Integer = -1
            For CurrentIndex As Integer = 0 To Me.Length - 1
                Dim CurrentObject As Object = Me.InternalArray(CurrentIndex)

                If (CurrentObject Is TargetObject) Then
                    TargetObjectIndex = CurrentIndex
                    Exit For
                End If
            Next

            REM No Good?
            If (TargetObjectIndex = -1) Then
                Return
            End If

            REM Update the Array
            For CurrentIndex As Integer = TargetObjectIndex To Me.InternalArray.Length
                If (CurrentIndex + 1 = Me.InternalArray.Length) Then
                    Exit For
                End If

                Me.InternalArray(CurrentIndex) = Me.InternalArray(CurrentIndex + 1)
            Next

            Me.Length -= 1
        End Sub

        REM Extends the Array size by one and sticks NewObject on the end.
        Public Function Append(NewObject As Object) As Integer
            Me.InternalArray(Me.InternalArray.Length - 1) = NewObject
            Me.Length += 1
            Return Me.InternalArray.Length - 2
        End Function

        Public Overrides Function ToString() As String
            Return "<DynamicArray>"
        End Function

        Public Sub Dispose()
            Me.IterationBegin(False)
            While (Not Me.IterationEnd())
                Me.IterationNext().Dispose()
            End While

            ReDim Me.InternalArray(0)

            Me.IsIterating = False
        End Sub
    End Class
End Namespace