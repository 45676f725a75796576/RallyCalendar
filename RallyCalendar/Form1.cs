using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RallyCalendar
{
    public partial class Form1 : Form
    {
        private const string EventsFile = "..\\..\\..\\events.json";
        private const string MessagesFile = "..\\..\\..\\messages.json";
        private Panel topBar;
        private Button calendarButton;
        private Button weatherButton;
        private Panel calendarPanel;
        private Button prevMonthButton;
        private Button nextMonthButton;
        private Label monthYearLabel;
        private TableLayoutPanel daysPanel;
        private Panel weatherPanel;
        private Label weatherLabel;
        private Panel bottomBar;
        private Label messageLabel;
        private int currentYear;
        private int currentMonth;
        private string BGImagePath = "..\\..\\..\\bg-image.jpg";


        private Weather w;

        // Flat list of events
        private List<CalendarEvent> allEvents = new();
        private List<string> messages = new();

        public Form1()
        {
            w = new Weather();

            InitializeWeatherPanel();
            InitializeDaysPanel();
            InitializeCalendarMenu();
            InitializeTopBar();
            InitializeBottomBar();

            this.Width = 680;
            this.Height = 480;

            this.BackgroundImage = Image.FromFile(BGImagePath);
            this.BackgroundImageLayout = ImageLayout.Stretch;

            this.Text = "Rally Calendar";

            LoadEventsFromFile();
            LoadMessagesFromFile();
            ShowRandomMessage();

            Controls.SetChildIndex(weatherPanel, 0);
            Controls.SetChildIndex(daysPanel, 1);
            Controls.SetChildIndex(calendarPanel, 2);
            Controls.SetChildIndex(topBar, 3);
            Controls.SetChildIndex(bottomBar, 4);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await w.GetWeather();
            weatherLabel.Text = w.WeatherString;
        }

        private void InitializeTopBar()
        {
            topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40
            };
            calendarButton = new Button
            {
                Text = "Calendar",
                Left = 10,
                Top = 5,
                Width = 100
            };
            calendarButton.Click += (s, e) => ShowCalendarMenu();

            weatherButton = new Button
            {
                Text = "Weather",
                Left = 120,
                Top = 5,
                Width = 100
            };
            weatherButton.Click += (s, e) => ShowWeatherPanel();

            topBar.Controls.Add(calendarButton);
            topBar.Controls.Add(weatherButton);
            Controls.Add(topBar);
        }

        private void InitializeBottomBar()
        {
            bottomBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = System.Drawing.Color.LightGray
            };
            messageLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Italic)
            };
            bottomBar.Controls.Add(messageLabel);
            Controls.Add(bottomBar);
        }

        private void InitializeCalendarMenu()
        {
            calendarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Visible = false
            };

            prevMonthButton = new Button
            {
                Text = "<",
                Left = 10,
                Top = 15,
                Width = 30
            };
            prevMonthButton.Click += (s, e) => ChangeMonth(-1);

            nextMonthButton = new Button
            {
                Text = ">",
                Left = 210,
                Top = 15,
                Width = 30
            };
            nextMonthButton.Click += (s, e) => ChangeMonth(1);

            monthYearLabel = new Label
            {
                Left = 60,
                Top = 20,
                Width = 140,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            calendarPanel.Controls.Add(prevMonthButton);
            calendarPanel.Controls.Add(monthYearLabel);
            calendarPanel.Controls.Add(nextMonthButton);
            Controls.Add(calendarPanel);

            currentYear = DateTime.Now.Year;
            currentMonth = DateTime.Now.Month;
            UpdateMonthYearLabel();
        }

        private void InitializeDaysPanel()
        {
            daysPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 7,
                Padding = new Padding(10),
                Visible = true
            };
            for (int i = 0; i < 7; i++)
            {
                daysPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28f));
            }
            Controls.Add(daysPanel);
            UpdateDaysPanel();
        }

        private async Task InitializeWeatherPanel()
        {
            weatherPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
            weatherLabel = new Label
            {
                Text = "Weather loading...",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            weatherPanel.Controls.Add(weatherLabel);
            Controls.Add(weatherPanel);
        }

        private void ShowCalendarMenu()
        {
            bool show = !calendarPanel.Visible;
            calendarPanel.Visible = show;
            daysPanel.Visible = show;
            weatherPanel.Visible = false;
            UpdateMonthYearLabel();
        }

        private void ShowWeatherPanel()
        {
            weatherPanel.Visible = true;
            daysPanel.Visible = false;
            calendarPanel.Visible = false;
        }

        private void ChangeMonth(int delta)
        {
            currentMonth += delta;
            if (currentMonth < 1)
            {
                currentMonth = 12;
                currentYear--;
            }
            else if (currentMonth > 12)
            {
                currentMonth = 1;
                currentYear++;
            }
            UpdateMonthYearLabel();
            UpdateDaysPanel();
        }

        private void UpdateMonthYearLabel()
        {
            monthYearLabel.Text = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentMonth)} {currentYear}";
        }

        private void UpdateDaysPanel()
        {
            daysPanel.Controls.Clear();
            daysPanel.RowStyles.Clear();

            int days = DateTime.DaysInMonth(currentYear + 1, currentMonth + 1);
            int rows = (int)Math.Ceiling(days / 7.0);
            daysPanel.RowCount = rows;

            for (int i = 0; i < rows; i++)
            {
                daysPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            }

            for (int i = 0; i < days; i++)
            {
                var dayButton = new Button
                {
                    Text = (i + 1).ToString(),
                    Width = 40,
                    Height = 40,
                    Margin = new Padding(5)
                };
                dayButton.Click += DayButton_Click;
                daysPanel.Controls.Add(dayButton, i % 7, i / 7);
            }
        }

        private void DayButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Text, out int dayNumber))
            {
                int dayIndex = dayNumber - 1;
                if (dayIndex >= 0 && dayIndex < DateTime.DaysInMonth(currentYear, currentMonth))
                {
                    ShowDayEventsDialog(dayIndex);
                }
            }
        }

        private void ShowDayEventsDialog(int dayIndex)
        {
            int day = dayIndex + 1;
            var eventsForDay = allEvents.FindAll(ev =>
                ev.Day == day && ev.Month == currentMonth && ev.Year == currentYear);

            using (Form dialog = new Form())
            {
                dialog.Text = $"Events for {day} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentMonth)}";
                dialog.Width = 350;
                dialog.Height = 300;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                ListBox eventsList = new ListBox
                {
                    Dock = DockStyle.Top,
                    Height = 180
                };
                foreach (var ev in eventsForDay)
                {
                    eventsList.Items.Add(ev.ToString());
                }

                eventsList.DoubleClick += (s, e) =>
                {
                    int idx = eventsList.SelectedIndex;
                    if (idx >= 0 && idx < eventsForDay.Count)
                    {
                        if (ShowEditEventDialog(eventsForDay[idx]))
                        {
                            eventsList.Items[idx] = eventsForDay[idx].ToString();
                            SaveEventsToFile();
                        }
                    }
                };

                TextBox newEventBox = new TextBox
                {
                    Dock = DockStyle.Top,
                    PlaceholderText = "New event"
                };

                Button addButton = new Button
                {
                    Text = "Add Event",
                    Dock = DockStyle.Top,
                    Height = 30
                };
                addButton.Click += (s, e) =>
                {
                    string text = newEventBox.Text.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        var newEvent = new CalendarEvent
                        {
                            Name = text,
                            Description = "",
                            Hours = 0,
                            Minutes = 0,
                            Day = day,
                            Month = currentMonth,
                            Year = currentYear
                        };
                        allEvents.Add(newEvent);
                        eventsList.Items.Add(newEvent.ToString());
                        newEventBox.Clear();
                        SaveEventsToFile();
                    }
                };

                dialog.Controls.Add(addButton);
                dialog.Controls.Add(newEventBox);
                dialog.Controls.Add(eventsList);

                dialog.ShowDialog(this);
            }
        }

        private bool ShowEditEventDialog(CalendarEvent ev)
        {
            using (Form editDialog = new Form())
            {
                editDialog.Text = "Edit Event";
                editDialog.Width = 320;
                editDialog.Height = 320;
                editDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                editDialog.StartPosition = FormStartPosition.CenterParent;
                editDialog.MaximizeBox = false;
                editDialog.MinimizeBox = false;

                Label nameLabel = new Label { Text = "Name:", Left = 10, Top = 15, Width = 60 };
                TextBox nameBox = new TextBox { Left = 80, Top = 10, Width = 200, Text = ev.Name };

                Label descLabel = new Label { Text = "Description:", Left = 10, Top = 50, Width = 80 };
                TextBox descBox = new TextBox { Left = 10, Top = 70, Width = 270, Height = 40, Multiline = true, Text = ev.Description };

                Label timeLabel = new Label { Text = "Time (hh:mm):", Left = 10, Top = 120, Width = 100 };
                NumericUpDown hourBox = new NumericUpDown { Left = 110, Top = 120, Width = 40, Minimum = 0, Maximum = 23, Value = ev.Hours };
                NumericUpDown minBox = new NumericUpDown { Left = 160, Top = 120, Width = 40, Minimum = 0, Maximum = 59, Value = ev.Minutes };

                Label dayLabel = new Label { Text = "Day:", Left = 10, Top = 160, Width = 40 };
                NumericUpDown dayBox = new NumericUpDown { Left = 60, Top = 160, Width = 50, Minimum = 1, Maximum = 31, Value = ev.Day };

                Label monthLabel = new Label { Text = "Month:", Left = 120, Top = 160, Width = 50 };
                NumericUpDown monthBox = new NumericUpDown { Left = 180, Top = 160, Width = 50, Minimum = 1, Maximum = 12, Value = ev.Month };

                Label yearLabel = new Label { Text = "Year:", Left = 10, Top = 200, Width = 40 };
                NumericUpDown yearBox = new NumericUpDown { Left = 60, Top = 200, Width = 80, Minimum = 1900, Maximum = 2100, Value = ev.Year };

                Button okButton = new Button { Text = "OK", Left = 40, Top = 240, Width = 70, DialogResult = DialogResult.OK };
                Button cancelButton = new Button { Text = "Cancel", Left = 120, Top = 240, Width = 70, DialogResult = DialogResult.Cancel };
                Button deleteButton = new Button { Text = "Delete", Left = 200, Top = 240, Width = 70 };

                editDialog.Controls.Add(nameLabel);
                editDialog.Controls.Add(nameBox);
                editDialog.Controls.Add(descLabel);
                editDialog.Controls.Add(descBox);
                editDialog.Controls.Add(timeLabel);
                editDialog.Controls.Add(hourBox);
                editDialog.Controls.Add(minBox);
                editDialog.Controls.Add(dayLabel);
                editDialog.Controls.Add(dayBox);
                editDialog.Controls.Add(monthLabel);
                editDialog.Controls.Add(monthBox);
                editDialog.Controls.Add(yearLabel);
                editDialog.Controls.Add(yearBox);
                editDialog.Controls.Add(okButton);
                editDialog.Controls.Add(cancelButton);
                editDialog.Controls.Add(deleteButton);

                editDialog.AcceptButton = okButton;
                editDialog.CancelButton = cancelButton;

                bool deleted = false;
                deleteButton.Click += (s, e) =>
                {
                    deleted = true;
                    editDialog.DialogResult = DialogResult.Abort;
                    editDialog.Close();
                };

                var result = editDialog.ShowDialog(this);

                if (deleted)
                {
                    allEvents.Remove(ev);
                    SaveEventsToFile();
                    return false; // Indicate deletion
                }

                if (result == DialogResult.OK)
                {
                    ev.Name = nameBox.Text;
                    ev.Description = descBox.Text;
                    ev.Hours = (int)hourBox.Value;
                    ev.Minutes = (int)minBox.Value;
                    ev.Day = (int)dayBox.Value;
                    ev.Month = (int)monthBox.Value;
                    ev.Year = (int)yearBox.Value;
                    SaveEventsToFile();
                    return true;
                }
                return false;
            }
        }

        private void SaveEventsToFile()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(EventsFile, JsonSerializer.Serialize(allEvents, options));
        }

        private void LoadEventsFromFile()
        {
            if (File.Exists(EventsFile))
            {
                try
                {
                    var loaded = JsonSerializer.Deserialize<List<CalendarEvent>>(File.ReadAllText(EventsFile));
                    if (loaded != null)
                        allEvents = loaded;
                }
                catch
                {
                    allEvents = new List<CalendarEvent>();
                }
            }
        }

        private void LoadMessagesFromFile()
        {
            if (File.Exists(MessagesFile))
            {
                try
                {
                    var json = File.ReadAllText(MessagesFile);
                    var doc = JsonDocument.Parse(json);
                    messages = doc.RootElement.EnumerateArray()
                        .Select(e => e.GetProperty("message").GetString() ?? "")
                        .Where(m => !string.IsNullOrWhiteSpace(m))
                        .ToList();
                }
                catch
                {
                    messages = new List<string>();
                }
            }
        }

        private void ShowRandomMessage()
        {
            if (messages.Count > 0)
            {
                var rnd = new Random();
                messageLabel.Text = messages[rnd.Next(messages.Count)];
            }
            else
            {
                messageLabel.Text = "No messages available.";
            }
        }
    }

    public class CalendarEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Hours:D2}:{Minutes:D2})";
        }
    }
}
