using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Xml.Linq;
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
            List<String> buttons = GetNumberPadNumbers(number);
            foreach(String button in buttons)
            {
                AutomationElement numberButton = mainWindow.FindFirst(TreeScope.Descendants,
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

        [When(@"I select Difference Between Dates")]
        public void WhenISelectDifferenceBetweenDates()
        {
            AutomationElement mainWindow = GetMainElement();
            AutomationElement comboSelector = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "DateCalculationOption"));
            ExpandComboBox(comboSelector);

            AutomationElement comboBoxItem = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, "Difference between dates"));
            SelectComboBoxItem(comboBoxItem);
        }

        [When(@"I Select (.*) of (.*) in (.*) DateSelector")]
        public void WhenISelectOfInDateSelector(string day, string month, string DateType)
        {
            AutomationElement datePicker = null;
            Thread.Sleep(500);
            AutomationElement mainWindow = GetMainElement();
            if(DateType == "StartDate")
            {
                datePicker = mainWindow.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "DateDiff_FromDate"));
            }
            else if( DateType == "EndDate")
            {
                datePicker = mainWindow.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "DateDiff_ToDate"));
            }
            ClickButton(datePicker);
            Thread.Sleep(500);
            AutomationElement yearPicker = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "HeaderButton"));
            ClickButton(yearPicker);
            Thread.Sleep(500);
            ClickButton(datePicker);
            AutomationElement monthPicker = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, month));
            ClickButton(monthPicker);

            ClickButton(datePicker);
            if (DateType == "StartDate")
            {
                SelectCalenderDayForAMonth(datePicker, mainWindow, day, 1);
            }
            else if (DateType == "EndDate")
            {
                SelectCalenderDayForAMonth(datePicker, mainWindow, day, 0);
            }


        }


        [Then(@"Day Difference shows (.*)")]
        public void ThenDayDifferenceShows(String numberOfDays)
        {
            AutomationElement mainWindow = GetMainElement();
            AutomationElement display = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, numberOfDays));

            if (display == null)
            {
                Assert.Fail("Mismatch in Number of days");
            }

        }

        [Then(@"Week Difference shows (.*)")]
        public void ThenWeekDifferenceShows(String WeekDifference)
        {
            AutomationElement mainWindow = GetMainElement();
            string result = "";
            AutomationElement display = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "DateDiffAllUnitsResultLabel"));

            if (display != null)
            {
                result = display.Current.Name;
            }

            Assert.IsTrue(result.Contains(WeekDifference), "Wrong results");
        }

        [When(@"I want to do (.*) calculation")]
        public void WhenIWantToDoCalculation(String calculation)
        {
            AutomationElement mainWindow = GetMainElement();
            String button = GetNumberStandardModeFunctionsItems(calculation);
            AutomationElement numberButton = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, button));
            if (numberButton != null)
            {
                var invokePattern = (InvokePattern)numberButton.GetCurrentPattern(InvokePattern.Pattern);
                invokePattern.Invoke();
            }
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
                {'0',"Zero" },
                {'=',"Equals" }
                
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

        public String GetNumberStandardModeFunctionsItems(string function)
        {
            Dictionary<string, String> functionButtonValues = new Dictionary<string, String>()
            {
                {"ADDITION","Plus" },
                {"SUBSTRATION","Minus" },
                {"DIVISION", "Divide by" },
                {"MULTIPLICATION", "Multiply by" },
                {"SQUARE", "Square" },
                {"SQUARE_ROOT", "Square root" },
                {"INVERSION","Reciprocal" }

            };
            return functionButtonValues[function];
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

        public void ExpandComboBox(AutomationElement element)
        {
            var expandCollapsePattern = element.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            if (expandCollapsePattern != null)
            {
                expandCollapsePattern.Expand();
            }
        }
        public void SelectComboBoxItem(AutomationElement element)
        {
            var selectionItemPattern = element.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
            if (selectionItemPattern != null)
            {
                selectionItemPattern.Select();
            }
        }

        public void ClickButton(AutomationElement element)
        {
            var invokePattern = (InvokePattern)element.GetCurrentPattern(InvokePattern.Pattern);
            invokePattern.Invoke();
        }

        public void ClickSelectButton(AutomationElement element)
        {
            var invokePattern = (SelectionItemPattern)element.GetCurrentPattern(SelectionItemPattern.Pattern);
            invokePattern.Select();
        }

        public void SelectCalenderDayForAMonth(AutomationElement datePicker, AutomationElement mainWindow, string day, int flag)
        {
            AutomationElement calView = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "CalendarView"));
            ClickButton(datePicker);

            Thread.Sleep(2000);
            AutomationElementCollection dataItems = calView.FindAll(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));
            foreach (AutomationElement dataItem in dataItems)
            {
                string dateText = dataItem.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
                if (dateText.Equals(day))
                {
                    SelectionItemPattern selectionPattern = dataItem.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                    if (selectionPattern != null)
                    {
                        selectionPattern.Select();
                        if(flag==1)
                            break;
                    }
                }
            }
        }
    }
}
