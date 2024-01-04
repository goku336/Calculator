using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CalculatorPOS
{
    public partial class FrmCalculatorPOS : DevExpress.XtraEditors.XtraForm
    {
        private List<SimpleButton> operationButtons = new List<SimpleButton>();
        private string currentInput = "";
        private double result = 0;
        //private char lastOperator = ' ';
        private bool isOperatorClicked = false;              // Added to prevent multiple operator clicks
        private List<SimpleButton> lstDigits { get; set; }

        public FrmCalculatorPOS()
        {
            InitializeComponent();
            txtDisplay.KeyPress += TxtDisplay_KeyPress;
            btnEqual.LookAndFeel.UseDefaultLookAndFeel = false;
            btnEqual.LookAndFeel.Style = LookAndFeelStyle.UltraFlat;
            btnEqual.ForeColor = Color.White;
            this.ActiveControl = txtDisplay;
        }   

        private void Button_Click(object sender, EventArgs e)
        {
            SimpleButton button = (SimpleButton)sender;
            string buttonText = button.Text;

            switch (buttonText)
            {
                case "C":
                    ClearAll();
                    break;
                case "⌫":
                    Backspace();
                    break;
                case "=":
                    if (!string.IsNullOrEmpty(currentInput))
                    {
                        EvaluateExpression();
                    }
                         break;                                                 
                   break;
                case "%":
                   ApplyPercentage();
                    break;
                case "/":
                case "*":
                case "-":
                case "+":
                    HandleOperationClick(buttonText);
                    break;
                default:
                    if (char.IsDigit(buttonText[0]) || buttonText == ".")
                    {
                        AppendInput(buttonText);
                    }
                    BackToFocus();
                    break;
            }
        }

        private void EvaluateExpression()
        {
            try
            {
                currentInput = currentInput.Trim();
                DataTable dataTable = new DataTable();
                if ("+-*/".Contains(currentInput.Last().ToString()))
                {
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
                var result = dataTable.Compute(currentInput, "");
                txtDisplay.Text = result.ToString();
                currentInput = result.ToString();

                // Reset isOperatorClicked to allow new operations
                isOperatorClicked = false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Invalid expression or calculation error.");
                // Clear the current input and display
                currentInput = "";
                txtDisplay.Text = "0";
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ClearAll()
        {
            currentInput = "";
            result = 0;
           // lastOperator = ' ';
            txtDisplay.Text = "0";
            BackToFocus();
        }

        private void Backspace()
        {
            if (currentInput.Length > 0)
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                txtDisplay.Text = currentInput;
                isOperatorClicked = false;
                BackToFocus();
            }
        }

        private void ApplyPercentage()
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                DataTable dataTable = new DataTable();
                // Handle any pending arithmetic operation before calculating the percentage
                if ("+-*/".Contains(currentInput.Last().ToString()))
                {
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
                var result = dataTable.Compute(currentInput, "");
                double inputNumber;
                if (double.TryParse(result.ToString(), out inputNumber))
                {
                    inputNumber /= 100;
                    currentInput = inputNumber.ToString();
                    txtDisplay.Text = currentInput;
                }
            }
        }


        private void HandleOperationClick(string operatorText)
        {
            if (!isOperatorClicked)
            {
                if (!string.IsNullOrEmpty(currentInput) && !currentInput.EndsWith(" "))
                {
                    currentInput += operatorText + "";
                    txtDisplay.Text = currentInput;
                    isOperatorClicked = true;
                    BackToFocus();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(currentInput))        //&& currentInput.EndsWith("+-*/"))
                {
                    string operators = "+-*/";
                    currentInput = currentInput.Substring(0, currentInput.Length - 1) + operatorText + "";
                    txtDisplay.Text = currentInput;
                   // isOperatorClicked = false;
                    BackToFocus();
                }

            }
        }

        private void BackToFocus()
        {
            txtDisplay.Focus();
            txtDisplay.SelectionStart = txtDisplay.Text.Length;
            txtDisplay.SelectionLength = 0;
        }

        private void AppendInput(string input)
        {
            isOperatorClicked = false;
            if ("+-*/".Contains(input))
            {
                if (!isOperatorClicked)
                {
                    if (!string.IsNullOrEmpty(currentInput) && !currentInput.EndsWith(" "))
                    {
                        currentInput += "" + input + "";
                        txtDisplay.Text = currentInput;
                        isOperatorClicked = true;
                    }
                }
            }
            else if (input == ".")
            {
                // Check if the last input is a decimal point or an operator
                if (!isOperatorClicked && !currentInput.EndsWith(".") && !currentInput.Contains("."))
                {
                    currentInput += ".";
                    txtDisplay.Text = currentInput;
                    isOperatorClicked = false;
                }
                else if (!isOperatorClicked && currentInput.Split("+-*/".ToCharArray()).Last().Contains(""))
                {
                    string tempValue;

                    tempValue = currentInput;
                    if (tempValue.Split("+-*/".ToCharArray()).Last().Contains("."))
                    {
                        return;
                    }
                    else
                    {
                        tempValue = currentInput + ".";
                        currentInput = tempValue;
                        txtDisplay.Text = currentInput;
                        isOperatorClicked = false;
                    }
                }
            }
            else if (input == "0" && currentInput == "0")
            {
                return;
            }
            else if (currentInput == "0" && input != ".")
            {
                currentInput = input;
                txtDisplay.Text = currentInput;
            }
            else if (input == "00" && currentInput == "00")
            {
                return;
            }
            else if (!isOperatorClicked && !"+-*/".Contains(input) && input == "00" && currentInput == "")
            {
                return;
            }
            //else if (!isOperatorClicked && currentInput.Split("+-*/".ToCharArray()).Last().Contains("") && input == "00" && currentInput == "")
            //{
            //    return;
            //}
            else if (currentInput == " ")
            {
                currentInput += input;
                txtDisplay.Text = currentInput;
            }
            else if (!isOperatorClicked && currentInput.Split("+-*/".ToCharArray()).Last().Contains(" "))
            {
                if (input == "00" && currentInput.EndsWith(""))
                {
                    return;
                }
                else if (input == "0" && currentInput.EndsWith("+-*/"))
                {
                    return;
                }
                else
                {
                    currentInput += input;
                    txtDisplay.Text = currentInput;
                    isOperatorClicked = false;
                }
            }
            else if (!isOperatorClicked)//|| !"+-*/".Contains(currentInput.Last().ToString()))
            {
                currentInput += input;
                txtDisplay.Text = currentInput;

                isOperatorClicked = false;
            }

            else if (!isOperatorClicked || currentInput == " ")
            {
                return;
            }          
        }
        private void FormateButtons()
        {
            try
            {
                lstDigits = new List<SimpleButton>();
                lstDigits.AddRange(new SimpleButton[] { btnDoubleZero, btnZero, btnOne, btnTwo, btnThree, btnFour, btnFive, btnSix, btnSeven, btnEight, btnNine,
                                                        btnBackspace, btnDot, btnMultiply, btnMinus, btnPlus, btnDivide, btnPercentage, btnClear, btnEqual });

                foreach (SimpleButton btn in lstDigits)
                {
                    btn.LookAndFeel.UseDefaultLookAndFeel = false;
                    btn.LookAndFeel.Style = LookAndFeelStyle.Flat;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void FrmCalculatorPOS_Load(object sender, EventArgs e)
        {
           // operationButtons.AddRange(new[] { btnPlus, btnMinus, btnMultiply, btnDivide });

            foreach (SimpleButton button in operationButtons)
            {
                button.Click += Button_Click;
            }

        }

        private void TxtDisplay_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;

            // Check if the key is a valid digit or operator
            if (char.IsDigit(keyChar)) // || "+-*/.".Contains(keyChar))
            {
                AppendInput(keyChar.ToString());
                BackToFocus();
            }
            else if (".".Contains(keyChar))
            {
               AppendInput(keyChar.ToString());
                BackToFocus();
            }
            else if ("+-*/.".Contains(keyChar))
            {
                HandleOperationClick(keyChar.ToString());
                BackToFocus();
            }
            else if (keyChar == '=' || keyChar == '\r') // '=' or Enter
            {
                EvaluateExpression();
                BackToFocus();

            }
            else if (keyChar == '\b') // Backspace
            {
                Backspace();
                BackToFocus();

            }
            else if (keyChar == (char)Keys.Escape) // Clear
            {
                ClearAll();
                BackToFocus();

            }
            // BackToFocus();          
            e.Handled = true;
        }
             
    }
}





