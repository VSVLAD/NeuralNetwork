Imports NeuralProject


Module Program

    Public Sub Main()

        Dim NN As New Network(2, 6, 6, 1)
        NN.LeaningRate = 0.1

        ' Обучаем
        For Each Epoch In Enumerable.Range(1, 20000000)
            NN.AverageQuadError = 0.0

            NN.TrainingSet({0.014, 0.007}, {0.049})
            NN.TrainingSet({0.011, 0.022}, {0.121})
            NN.TrainingSet({0.011, 0.024}, {0.132})
            NN.TrainingSet({0.022, 0.023}, {0.253})
            NN.TrainingSet({0.016, 0.013}, {0.104})
            NN.TrainingSet({0.022, 0.011}, {0.121})
            NN.TrainingSet({0.02, 0.017}, {0.17})
            NN.TrainingSet({0.019, 0.022}, {0.209})
            NN.TrainingSet({0.02, 0.008}, {0.08})
            NN.TrainingSet({0.018, 0.022}, {0.198})
            NN.TrainingSet({0.024, 0.018}, {0.216})
            NN.TrainingSet({0.016, 0.022}, {0.176})
            NN.TrainingSet({0.025, 0.024}, {0.3})
            NN.TrainingSet({0.015, 0.007}, {0.0525})
            NN.TrainingSet({0.015, 0.016}, {0.12})
            NN.TrainingSet({0.025, 0.022}, {0.275})
            NN.TrainingSet({0.011, 0.017}, {0.0935})
            NN.TrainingSet({0.019, 0.023}, {0.2185})
            NN.TrainingSet({0.017, 0.011}, {0.0935})
            NN.TrainingSet({0.014, 0.014}, {0.098})

            NN.AverageQuadError = NN.AverageQuadError / 20
            If NN.AverageQuadError < 0.0000001 Then Exit For

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", Epoch, NN.AverageQuadError.ToString("##0.##########"))

            ' Управление
            If Console.KeyAvailable Then
                Select Case Console.ReadKey().Key
                    Case ConsoleKey.Z
                        Exit For
                    Case ConsoleKey.S
                        NN.LeaningRate += 0.01
                        Console.Title = "LeaningRate: " & NN.LeaningRate
                    Case ConsoleKey.A
                        NN.LeaningRate -= 0.01
                        Console.Title = "LeaningRate: " & NN.LeaningRate
                End Select
            End If
        Next

        ' Тестируем по сетам
        Console.WriteLine("=== Проверка работы сети ===")
        Console.WriteLine("Проверка на значениях: 20 и 9. Ожидаем 90. Решение {0}", NN.Predict({0.02, 0.009})(0) * 1000)
        Console.WriteLine("Проверка на значениях: 10 и 17. Ожидаем 85. Решение {0}", NN.Predict({0.01, 0.017})(0) * 1000)
        Console.ReadLine()
    End Sub


    Public Sub TaskXOR()

        ' 0  0    0
        ' 0  1    1
        ' 1  0    1
        ' 1  1    0

        Dim NN As New Network(2, 4, 1)
        NN.LeaningRate = 0.005

        ' Обучаем
        For Each Epoch In Enumerable.Range(1, 50000)
            NN.AverageQuadError = 0.0

            NN.TrainingSet({0, 0}, {0})
            NN.TrainingSet({0, 1}, {1})
            NN.TrainingSet({1, 0}, {1})
            NN.TrainingSet({1, 1}, {0})

            NN.AverageQuadError = NN.AverageQuadError / 4
            If NN.AverageQuadError < 0.001 Then Exit For

            Console.WriteLine("Эпоха: {0}. Ошибка {1}", Epoch, NN.AverageQuadError.ToString("##0.##########"))
        Next

        ' Тестируем по сетам
        Console.WriteLine("=== Проверка работы сети ===")
        Console.WriteLine("Проверка на значениях: 0 и 0. Ожидаем 0. Решение {0}", NN.Predict({0, 0})(0))
        Console.WriteLine("Проверка на значениях: 0 и 1. Ожидаем 1. Решение {0}", NN.Predict({0, 1})(0))
        Console.WriteLine("Проверка на значениях: 1 и 0. Ожидаем 1. Решение {0}", NN.Predict({1, 0})(0))
        Console.WriteLine("Проверка на значениях: 1 и 1. Ожидаем 0. Решение {0}", NN.Predict({1, 1})(0))
        Console.ReadLine()
    End Sub

End Module
