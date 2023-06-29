using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using static System.Net.Mime.MediaTypeNames;


namespace CalculatorTest.StepDefinitions
{
    [Binding]
    public class StepDefenition
    {
        [Given(@"I open CalulatorApps")]
        public void GivenIOpenCalulatorApps()
        {
            Process.Start("calc");
            Thread.Sleep(5000);
        }

        [When(@"I Select (.*) Mode")]
        public void WhenISelectMode(string mode)
        {
            AutomationElement mainWindow = GetMainElement();
            AutomationElement togglePaneButton = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty,"TogglePaneButton"));
            if(togglePaneButton != null)
            {
                var invokePatterns = (InvokePattern)togglePaneButton.GetCurrentPattern(InvokePattern.Pattern);
                invokePatterns.Invoke();
            }

            AutomationElement menuItems = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "MenuItemsHost"));
            AutomationElement listItem = GetListItemByName(menuItems, mode);

            var selectionItemPattern = (SelectionItemPattern)listItem.GetCurrentPattern(SelectionItemPattern.Pattern);
            selectionItemPattern.Select();
            Thread.Sleep(1000);
        }

        [When(@"I enter (.*) in the NumberPad")]
        public void WhenIEnterInTheNumberPad(string number)
        {
            AutomationElement mainWindow = GetMainElement();
            AutomationElement numberPad = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "NumberPad"));
            List<String> buttons = GetNumberPadNumbers(number);
            foreach(String button in buttons)
            {
                AutomationElement numberButton = numberPad.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, button));
                if (numberButton != null)
                {
                    var invokePattern = (InvokePattern)numberButton.GetCurrentPattern(InvokePattern.Pattern);
                    invokePattern.Invoke();
                }
            }
            
        }

        [When(@"I Select (.*) Method For Calculation")]
        public void WhenISelectMethodForCalculation(String method)
        {
            AutomationElement mainWindow = GetMainElement();
            //AutomationElement numberPad = mainWindow.FindFirst(TreeScope.Descendants,
            //    new PropertyCondition(AutomationElement.NameProperty, "Scientific functions"));
            String button = GetNumberPadCalulationItems(method);
            AutomationElement numberButton = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, button));
            if (numberButton != null)
            {
                var invokePattern = (InvokePattern)numberButton.GetCurrentPattern(InvokePattern.Pattern);
                invokePattern.Invoke();
            }
        }


        [When(@"I select (.*) Selection")]
        public void WhenISelectSelection(string option)
        {
            string automationIdProperty = null;
            if (option == "HEX")
                automationIdProperty = "hexButton";
            else if (option == "DEC")
                automationIdProperty = "decimalButton";
            else if (option == "OCT")
                automationIdProperty = "octolButton";
            else if (option == "BIN")
                automationIdProperty = "binaryButton";

            AutomationElement mainWindow = GetMainElement();
            AutomationElement selectorRadio = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty,automationIdProperty));
            if(selectorRadio != null)
            {
                var selectRadioPattern = (SelectionItemPattern)selectorRadio.GetCurrentPattern(SelectionItemPattern.Pattern);
                selectRadioPattern.Select();
            }
        }

        [Then(@"Results should display (.*)")]
        public void ThenResultsShouldDisplay(string ExpectedValue)
        {
            AutomationElement mainWindow = GetMainElement();
            string result="";
            AutomationElement display = mainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "CalculatorResults"));

            if (display != null)
            {
                result = display.Current.Name.Trim("Display is".ToCharArray()).Replace(" ", string.Empty).Replace(",",string.Empty);
            }
        
            Assert.IsTrue(result.Contains(ExpectedValue),"Wrong results");
        }



        public AutomationElement GetMainElement()
        {
            return AutomationElement.RootElement.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.NameProperty, "Calculator"));
        }

        public AutomationElement GetListItemByName(AutomationElement listContainer, string itemId)
        {
            Condition condition = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem),
                new PropertyCondition(AutomationElement.AutomationIdProperty,itemId));
            return listContainer.FindFirst(TreeScope.Children, condition);

        }

        public List<String> GetNumberPadNumbers(string number)
        {
            List<String> buttons = new List<string>();
            char[] numbers = number.ToCharArray();
            Dictionary<char, String> numberButtonValues = new Dictionary<char, String>()
            {
                {'1',"One" },
                {'2',"Two" },
                {'3',"Three" },
                {'4',"Four" },
                {'5', "Five" },
                {'6', "Six" },
                {'7',"Seven" },
                {'8', "Eight" },
                {'9',"Nine" },
                {'0',"Zero" }
            };
            
            foreach (char digit in numbers)
            {
                buttons.Add(numberButtonValues[digit]);
            }
            return buttons;
        }

        public String GetNumberPadCalulationItems(string Calculation)
        {
            Dictionary<string, String> numberButtonValues = new Dictionary<string, String>()
            {
                {"SQUARE","Square" },
                {"SQUARE_ROOT","Square root" },
                {"FACTORIAL", "Factorial" },
                {"LOG", "Log" },
                {"TEN_EXPONENT", "Ten to the exponent" }
               
            };
            return numberButtonValues[Calculation];
        }



        [AfterTestRun]
        public static void CloseCalculator()
        {
            Process[] process = Process.GetProcessesByName("CalculatorApp");
            foreach (Process processItem in process)
            {
                processItem.Kill();
            }
        }
    }
}
