﻿using System;
using System.Collections.Generic;

namespace DriverLicenseApp.DAL.Models;

public partial class Exam
{
    public int ExamId { get; set; }

    public int CourseId { get; set; }

    public DateOnly ExamDate { get; set; }

    public TimeOnly ExamTime { get; set; }

    public int DurationMinutes { get; set; }

    public string Room { get; set; } = null!;

    public int UserId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public virtual User User { get; set; } = null!;
}
