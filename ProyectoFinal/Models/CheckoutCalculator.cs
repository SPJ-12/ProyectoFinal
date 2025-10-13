using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinal.Models;

public class CheckoutCombination : INotifyPropertyChanged
{
    private string _displayText;
    private int _totalPoints;
    private bool _isPossible;

    public string DisplayText
    {
        get => _displayText;
        set
        {
            if (_displayText != value)
            {
                _displayText = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalPoints
    {
        get => _totalPoints;
        set
        {
            if (_totalPoints != value)
            {
                _totalPoints = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsPossible
    {
        get => _isPossible;
        set
        {
            if (_isPossible != value)
            {
                _isPossible = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public static class CheckoutCalculator
{
    // Números válidos para dardos (1-20, 25 para bullseye)
    private static readonly int[] VALID_NUMBERS = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 25 };
    
    // Marcas válidas (1 = simple, 2 = doble, 3 = triple)
    private static readonly int[] VALID_MARKS = { 1, 2, 3 };

    public static List<CheckoutCombination> GetCheckoutCombinations(int remainingScore)
    {
        var combinations = new List<CheckoutCombination>();

        if (remainingScore <= 0 || remainingScore > 170) // 170 es el máximo posible en 3 dardos
        {
            return combinations;
        }

        // Buscar combinaciones de 1, 2 y 3 dardos
        FindOneDartCheckouts(remainingScore, combinations);
        FindTwoDartCheckouts(remainingScore, combinations);
        FindThreeDartCheckouts(remainingScore, combinations);

        // Ordenar por prioridad: primero las más fáciles (menos dardos), luego por puntos totales
        return combinations.OrderBy(c => c.TotalPoints).ToList();
    }

    private static void FindOneDartCheckouts(int remainingScore, List<CheckoutCombination> combinations)
    {
        foreach (var number in VALID_NUMBERS)
        {
            foreach (var marks in VALID_MARKS)
            {
                int points = number * marks;
                if (points == remainingScore)
                {
                    // Verificar que el cierre sea válido (debe terminar en doble)
                    if (marks == 2 || (number == 25 && marks == 1)) // Bull exterior cuenta como doble
                    {
                        combinations.Add(new CheckoutCombination
                        {
                            DisplayText = GetDartDisplayText(number, marks),
                            TotalPoints = points,
                            IsPossible = true
                        });
                    }
                }
            }
        }
    }

    private static void FindTwoDartCheckouts(int remainingScore, List<CheckoutCombination> combinations)
    {
        for (int i = 0; i < VALID_NUMBERS.Length; i++)
        {
            for (int j = 0; j < VALID_MARKS.Length; j++)
            {
                int firstDart = VALID_NUMBERS[i] * VALID_MARKS[j];
                
                for (int k = 0; k < VALID_NUMBERS.Length; k++)
                {
                    for (int l = 0; l < VALID_MARKS.Length; l++)
                    {
                        int secondDart = VALID_NUMBERS[k] * VALID_MARKS[l];
                        int total = firstDart + secondDart;
                        
                        if (total == remainingScore)
                        {
                            // El segundo dardo debe ser un doble para cerrar
                            if (VALID_MARKS[l] == 2 || (VALID_NUMBERS[k] == 25 && VALID_MARKS[l] == 1))
                            {
                                combinations.Add(new CheckoutCombination
                                {
                                    DisplayText = $"{GetDartDisplayText(VALID_NUMBERS[i], VALID_MARKS[j])} + {GetDartDisplayText(VALID_NUMBERS[k], VALID_MARKS[l])}",
                                    TotalPoints = total,
                                    IsPossible = true
                                });
                            }
                        }
                    }
                }
            }
        }
    }

    private static void FindThreeDartCheckouts(int remainingScore, List<CheckoutCombination> combinations)
    {
        for (int i = 0; i < VALID_NUMBERS.Length; i++)
        {
            for (int j = 0; j < VALID_MARKS.Length; j++)
            {
                int firstDart = VALID_NUMBERS[i] * VALID_MARKS[j];
                
                for (int k = 0; k < VALID_NUMBERS.Length; k++)
                {
                    for (int l = 0; l < VALID_MARKS.Length; l++)
                    {
                        int secondDart = VALID_NUMBERS[k] * VALID_MARKS[l];
                        
                        for (int m = 0; m < VALID_NUMBERS.Length; m++)
                        {
                            for (int n = 0; n < VALID_MARKS.Length; n++)
                            {
                                int thirdDart = VALID_NUMBERS[m] * VALID_MARKS[n];
                                int total = firstDart + secondDart + thirdDart;
                                
                                if (total == remainingScore)
                                {
                                    // El tercer dardo debe ser un doble para cerrar
                                    if (VALID_MARKS[n] == 2 || (VALID_NUMBERS[m] == 25 && VALID_MARKS[n] == 1))
                                    {
                                        combinations.Add(new CheckoutCombination
                                        {
                                            DisplayText = $"{GetDartDisplayText(VALID_NUMBERS[i], VALID_MARKS[j])} + {GetDartDisplayText(VALID_NUMBERS[k], VALID_MARKS[l])} + {GetDartDisplayText(VALID_NUMBERS[m], VALID_MARKS[n])}",
                                            TotalPoints = total,
                                            IsPossible = true
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static string GetDartDisplayText(int number, int marks)
    {
        if (number == 25)
        {
            return marks == 1 ? "Bull" : "Bull";
        }
        
        return marks switch
        {
            1 => number.ToString(),
            2 => $"D{number}",
            3 => $"T{number}",
            _ => $"{number}x{marks}"
        };
    }

    public static bool CanCheckout(int remainingScore)
    {
        return GetCheckoutCombinations(remainingScore).Any();
    }
}

