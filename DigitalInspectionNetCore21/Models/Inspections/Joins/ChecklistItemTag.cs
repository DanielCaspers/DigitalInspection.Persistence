﻿using System;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
    public class ChecklistItemTag
    {
	    public Guid ChecklistItemId { get; set; }

	    public ChecklistItem ChecklistItem { get; set; }

	    public Guid TagId { get; set; }

	    public Tag Tag { get; set; }
	}
}