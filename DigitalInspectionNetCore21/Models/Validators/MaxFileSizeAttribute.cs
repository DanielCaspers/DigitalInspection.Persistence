using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DigitalInspectionNetCore21.Models.Validators
{
	public class MaxFileSizeAttribute : ValidationAttribute
	{
		private readonly int _maxFileSize;
		public MaxFileSizeAttribute(int maxFileSize)
		{
			_maxFileSize = maxFileSize;
		}

		public override bool IsValid(object value)
		{
			if (value is IFormFile file)
			{
				return file.Length <= _maxFileSize;
			}
			return false;
		}

		public override string FormatErrorMessage(string name)
		{
			return base.FormatErrorMessage(_maxFileSize.ToString());
		}
	}
}
