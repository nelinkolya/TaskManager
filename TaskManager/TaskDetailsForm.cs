﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using TaskManager.entity;

namespace TaskManager
{
    public partial class TaskDetailsForm : Form
    {
        private readonly Task _currentTask;
        private readonly Action<Task> _updateTaskListDelegate;

        public TaskDetailsForm(Task task, Action<Task> updateTaskListDelegate = null)
        {
            InitializeComponent();

            comboBoxCategory.DataSource = EnumHelper<ECategory>.GetDisplayValues(ECategory.Others);

            this._updateTaskListDelegate = updateTaskListDelegate;
            _currentTask = task;

            textBoxName.Text = task.Name;
            dateTimePickerTimeOfTask.Value = task.Date;
            comboBoxCategory.TabIndex = (int)task.Category;
            textBoxDescription.Text = task.Description;
            checkBoxActivityStatus.Checked = task.IsActive;
        }

        private void buttonSaveTask_Click(object sender, EventArgs e)
        {
            _currentTask.Date = dateTimePickerTimeOfTask.Value;
            _currentTask.IsActive = checkBoxActivityStatus.Checked;
            _currentTask.Name = textBoxName.Text;
            _currentTask.Category = (ECategory)comboBoxCategory.TabIndex;
            _currentTask.Description = textBoxDescription.Text;

            if(_updateTaskListDelegate != null)
            {
                _updateTaskListDelegate(_currentTask);
            }

            this.Hide();
        }
    }
}
