Imports NeuralProject

Public Class Form1

    Dim vis As New NeuralVisualizer


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' 0  0    0
        ' 0  1    1
        ' 1  0    1
        ' 1  1    0

        Dim NN As New NeuralNetwork(2, 4, 1)
        Dim LeaningRate = 0.2
        Dim AvgQuadError = 0.0

        If Not vis.IsInitialized() Then vis.InitRender(pbox, NN)

        ' Обучаем
        For Each Epoch In Enumerable.Range(1, 50000)
            AvgQuadError = 0.0
            AvgQuadError += NN.Training({0, 0}, {0}, LeaningRate)
            AvgQuadError += NN.Training({0, 1}, {1}, LeaningRate)
            AvgQuadError += NN.Training({1, 0}, {1}, LeaningRate)
            AvgQuadError += NN.Training({1, 1}, {0}, LeaningRate)

            AvgQuadError = AvgQuadError / 4
            If AvgQuadError < 0.000001 Then Exit For
        Next
    End Sub
End Class
