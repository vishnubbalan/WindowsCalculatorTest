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
using TechTalk.SpecFlow.CommonModels;
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


        [Given(@"I open CalculatorApp in full window")]
        public void GivenIOpenCalculatorAppInFullWindow()
        {
            Process.Start("calc");
            Thread.Sleep(5000);
            Condition condition = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                new PropertyCondition(AutomationElement.NameProperty, "Calculator"));
            AutomationElement element = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, condition);
            WindowPattern windowPattern = (WindowPattern)element.GetCurrentPattern(WindowPattern.Pattern);
            if (windowPattern != null)
            {
                windowPattern.SetWindowVisualState(WindowVisualState.Maximized);
            }
        }


        [When(@"I Select (.*) Mode")]
        public void WhenISelectMode(string mode)
        {
            AutomationElement mainWindow = GetMainElement();
            AutomationElement togglePaneButton = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "TogglePaneButton"));
            if (togglePaneButton != null)
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

        [When(@"I want to do addition for following")]
        public void WhenIWantToDoAdditionForFollowing(Table table)
        {
            foreach (var row in table.Rows)
            {
                string param1 = row[0].ToString();
                string param2 = row[1].ToString();
                string result = row[2].ToString();
                WhenIEnterInTheNumberPad(param1);
                WhenIWantToDoCalculation("ADDITION");
                WhenIEnterInTheNumberPad(param2);
                WhenIEnterInTheNumberPad("=");
            }
        }

        [When(@"I want to do addition for following with memory addition")]
        public void WhenIWantToDoAdditionForFollowingWithMemoryAddition(Table table)
        {
            foreach (var row in table.Rows)
            {
                string param1 = row[0].ToString();
                string param2 = row[1].ToString();
                string result = row[2].ToString();
                WhenIEnterInTheNumberPad(param1);
                WhenIWantToDoCalculation("ADDITION");
                WhenIEnterInTheNumberPad(param2);
                WhenIEnterInTheNumberPad("=");
                StoreToMemory();
            }
        }


        [Then(@"I can retreive the following calculations from the history")]
        public void ThenICanRetreiveTheFollowingCalculationsFromTheHistory(Table table)
        {
            AutomationElement mainWindow = GetMainElement();
            AutomationElement historyList = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "HistoryListView"));
            AutomationElementCollection historyListItems = historyList.FindAll(TreeScope.Children,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));
            foreach (var row in table.Rows)
            {
                string history = row[0].ToString();
                AutomationElement historyItem = historyList.FindFirst(TreeScope.Children,
                    new PropertyCondition(AutomationElement.NameProperty, history));
                if (historyItem != null)
                {
                    InvokePattern invokePattern = (InvokePattern)historyItem.GetCurrentPattern(InvokePattern.Pattern);
                    if (invokePattern != null)
                        invokePattern.Invoke();
                }
                else
                {
                    AutomationElement textItem = GetListItemConatiningTextItem(historyListItems, row[1].ToString() + "=");
                    if (textItem != null)
                    {
                        InvokePattern invokePattern = (InvokePattern)textItem.GetCurrentPattern(InvokePattern.Pattern);
                        if (invokePattern != null)
                            invokePattern.Invoke();
                    }
                }


                string expression = GetTextElementContaineText("CalculatorExpression");
                Assert.IsTrue(expression.Contains(row[1].ToString()), "Wrong Expression");
                String result = GetTextElementContaineText("CalculatorResults");
                Assert.IsTrue(result.Contains(row[2].ToString()), "Wrong Result");

            }
        }

        [Then(@"I can retreive the following calculations from the memory")]
        public void ThenICanRetreiveTheFollowingCalculationsFromTheMemory(Table table)
        {

            AutomationElement mainWindow = GetMainElement();
            AutomationElement historyList = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "MemoryLabel"));
            SelectionItemPattern pattern = (SelectionItemPattern)historyList.GetCurrentPattern(SelectionItemPattern.Pattern);
            if (pattern != null)
            {
                pattern.Select();
            }

            AutomationElement memoryList = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "MemoryListView"));
            AutomationElementCollection memoryListItems = memoryList.FindAll(TreeScope.Children,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));
            foreach (var row in table.Rows)
            {
                string history = row[0].ToString();
                AutomationElement historyItem = memoryList.FindFirst(TreeScope.Children,
                    new PropertyCondition(AutomationElement.NameProperty, history));
                if (historyItem != null)
                {
                    InvokePattern invokePattern = (InvokePattern)historyItem.GetCurrentPattern(InvokePattern.Pattern);
                    if (invokePattern != null)
                        invokePattern.Invoke();
                }
                else
                {
                    AutomationElement textItem = GetListItemConatiningTextItem(memoryListItems, row[0].ToString());
                    if (textItem != null)
                    {
                        InvokePattern invokePattern = (InvokePattern)textItem.GetCurrentPattern(InvokePattern.Pattern);
                        if (invokePattern != null)
                            invokePattern.Invoke();
                    }
                }


                String result = GetTextElementContaineText("CalculatorResults");
                Assert.IsTrue(result.Contains(row[0].ToString()), "Wrong Result");

            }
        }



        [When(@"I enter (.*) in the NumberPad")]
        public void WhenIEnterInTheNumberPad(string number)
        {
            AutomationElement mainWindow = GetMainElement();
            List<String> buttons = GetNumberPadNumbers(number);
            foreach (String button in buttons)
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
                new PropertyCondition(AutomationElement.AutomationIdProperty, automationIdProperty));
            if (selectorRadio != null)
            {
                var selectRadioPattern = (SelectionItemPattern)selectorRadio.GetCurrentPattern(SelectionItemPattern.Pattern);
                selectRadioPattern.Select();
            }
        }

        [Then(@"Results should display (.*)")]
        public void ThenResultsShouldDisplay(string ExpectedValue)
        {
            AutomationElement mainWindow = GetMainElement();
            string result = "";
            AutomationElement display = mainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "CalculatorResults"));

            if (display != null)
            {
                result = display.Current.Name.Trim("Display is".ToCharArray()).Replace(" ", string.Empty).Replace(",", string.Empty);
            }

            Assert.IsTrue(result.Contains(ExpectedValue), "Wrong results");
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
            if (DateType == "StartDate")
            {
                datePicker = mainWindow.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "DateDiff_FromDate"));
            }
            else if (DateType == "EndDate")
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
                new PropertyCondition(AutomationElement.AutomationIdProperty, itemId));
            return listContainer.FindFirst(TreeScope.Children, condition);

        }

        public AutomationElement GetTextItemByName(AutomationElement listContainer, string name)
        {
            Condition condition = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text),
                new PropertyCondition(AutomationElement.NameProperty, name));
            return listContainer.FindFirst(TreeScope.Descendants, condition);

        }

        public AutomationElement GetListItemConatiningTextItem(AutomationElementCollection container, string text)
        {
            foreach (AutomationElement item in container)
            {
                AutomationElement textItem = item.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.NameProperty, text));
                if (textItem != null)
                {
                    return item;
                }

            }
            return null;


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
                        if (flag == 1)
                            break;
                    }
                }
            }
        }

        public string GetTextElementContaineText(string automationIdProperty)
        {
            AutomationElement mainWindow = GetMainElement();
            AutomationElement calculatorExpression = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, automationIdProperty));
            string text = "";
            if (calculatorExpression != null)
            {
                text = calculatorExpression.Current.Name;
            }
            return text;
        }

        public void StoreToMemory()
        {
            AutomationElement mainWindow = GetMainElement();
            AutomationElement AddToMemButton = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "memButton"));
            InvokePattern pattern = (InvokePattern)AddToMemButton.GetCurrentPattern(InvokePattern.Pattern);
            if(pattern != null)
            {
                pattern.Invoke();
            }
        }
    }
}