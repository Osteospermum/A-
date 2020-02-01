Module Module1

    Class coord
        Public Property X As Double
        Public Property Y As Double

        Public Sub New(ByVal x As Double, ByVal y As Double)
            Me.X = x
            Me.Y = y
        End Sub
    End Class

    Class tile
        Public Property start As Boolean
        Public Property goal As Boolean
        Public Property impassable As Boolean
        Public Property parents() As tile
        Public Property g As Integer
        Public Property h As Integer
        Public Property f As Integer
        Public Property coordinate As coord
    End Class

    Class map
        'Grid tiles sorted as x val, y val
        Public ReadOnly grid()() As tile
        Public openList() As tile
        Public closedList() As tile

        'Constructor (New map has specified grid size)
        Public Sub New(ByVal gridLength As Integer, Optional ByVal gridWidth As Integer = -1)
            If gridLength < 0 Then
                gridLength = 1
            End If
            If gridWidth < 0 Then
                gridWidth = gridLength
            End If
            'Initialise tiles
            ReDim grid(gridLength - 1)
            For i As Integer = 0 To gridLength - 1
                ReDim grid(i)(gridWidth - 1)
                For k As Integer = 0 To grid.Length - 1
                    grid(i)(k) = New tile
                    grid(i)(k).start = False
                    grid(i)(k).goal = False
                    grid(i)(k).impassable = False
                    grid(0)(0).start = True
                    grid(0)(0).goal = True
                    grid(i)(k).coordinate = New coord(i + 1, k + 1)
                Next
            Next
            updateH()
        End Sub

        'Make only specified tile start
        Public Sub setStart(ByVal x As Integer, ByVal y As Integer)
            If x <= grid.Length And y <= grid.Length And x > 0 And y > 0 Then
                For i As Integer = 0 To grid.Length - 1
                    For k As Integer = 0 To grid(i).Length - 1
                        grid(i)(k).start = False
                    Next
                Next
                grid(x - 1)(y - 1).start = True
            End If
            updateH()
        End Sub

        'Make only specified tile goal
        Public Sub setGoal(ByVal x As Integer, ByVal y As Integer)
            If x <= grid.Length And y <= grid.Length And x > 0 And y > 0 Then
                For i As Integer = 0 To grid.Length - 1
                    For k As Integer = 0 To grid(i).Length - 1
                        grid(i)(k).goal = False
                    Next
                Next
                grid(x - 1)(y - 1).goal = True
            End If
            updateH()
        End Sub

        'Switches the value of an impassable item
        Public Sub changeImpassable(ByVal x As Integer, ByVal y As Integer)
            If x <= grid.Length And y <= grid.Length And x > 0 And y > 0 Then
                grid(x - 1)(y - 1).impassable = Not grid(x - 1)(y - 1).impassable
            End If
            updateH()
        End Sub

        'Find Heuristic values
        Private Sub updateH()
            Dim goal As coord = getGoalTile().coordinate
            For i As Integer = 0 To grid.Length - 1
                For k As Integer = 0 To grid(i).Length - 1
                    If grid(i)(k).impassable = False And grid(i)(k).goal = False Then
                        grid(i)(k).h = Math.Abs((i + 1) - goal.X) + Math.Abs((k + 1) - goal.Y)
                    Else
                        grid(i)(k).h = 0
                    End If
                Next
            Next
        End Sub

        'Update the closed list to be only the starting point
        Private Sub resetLists()
            ReDim closedList(0)
            closedList(0) = getStartTile()
            updateOpenList()
        End Sub

        'Update parents, Gs and open list
        Private Sub updateOpenList()
            Dim current As tile
            If closedList.Length = 1 Then
                current = closedList(0)
            End If
        End Sub


#Region "Tile and coord gets"
        Public Function getStartTile()
            For i As Integer = 0 To grid.Length - 1
                For k As Integer = 0 To grid(i).Length - 1
                    If grid(i)(k).start = True Then
                        Return grid(i)(k)
                    End If
                Next
            Next
            Return grid(0)(0)
        End Function

        Public Function getGoalTile()
            For i As Integer = 0 To grid.Length - 1
                For k As Integer = 0 To grid(i).Length - 1
                    If grid(i)(k).goal = True Then
                        Return grid(i)(k)
                    End If
                Next
            Next
            Return grid(0)(0)
        End Function
#End Region
    End Class

    Public problem As New map(5)

    Sub Main()
        'Assign special tiles their attribute
        problem.setStart(1, 3)
        problem.setGoal(5, 4)
        problem.changeImpassable(4, 2)
        problem.changeImpassable(4, 4)
        problem.changeImpassable(2, 5)
        problem.changeImpassable(4, 5)
        For k As Integer = 0 To 4
            For i As Integer = 0 To 4
                If problem.grid(i)(k).start = True Then
                    Console.Write("X")
                ElseIf problem.grid(i)(k).goal = True Then
                    Console.Write("*")
                ElseIf problem.grid(i)(k).impassable = True Then
                    Console.Write("~")
                Else
                    Console.Write("O")
                End If
                Console.Write(" ")
            Next
            Console.WriteLine()
        Next
        For k As Integer = 0 To 4
            For i As Integer = 0 To 4
                Console.Write(problem.grid(i)(k).h)
                Console.Write(", ")
            Next
            Console.WriteLine()
        Next
        Console.ReadLine()
    End Sub

End Module
