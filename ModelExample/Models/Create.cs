using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModelExample.Models
{
    public class AgentAttribute : ValidationAttribute
    {
        private string Other { get; }
        public AgentAttribute(string other)
            => Other = other;
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(Other);
            if (property == null)
            {
                return new ValidationResult(
                    string.Format("Unknown property: {0}", Other)
                );
            }
            var otherValue = property.GetValue(validationContext.ObjectInstance, null);

            // _other = 請假者 otherValue = 代理人
            // 拿請假者去伺服器查詢比對代理人是否在清單內
            // 有在清單內回傳 true 沒在清單內回傳 false
            bool Valida = value.ToString() == "I116643";
            if (!Valida)
            {
                return new ValidationResult("請勿使用非法手段輸入代理人");
            }
            return ValidationResult.Success;
        }
    }
    public class TimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var create = (Create)validationContext.ObjectInstance;
            string time = value?.ToString();
            // 驗證時間 是否在清單內
            bool Valida = create.SelectTime.Where(x => x.Value == time).Count() > 0;
            if (!Valida)
            {
                return new ValidationResult("請勿使用非法手段輸入時間");
            }
            return ValidationResult.Success;
        }
    }
    public class Create
    {
        [Required]
        [StringLength(7, MinimumLength = 0)]
        public string Emp_id { get; set; }
        [Required]
        [Agent("Emp_id")]
        [Display(Name = "代理人")]
        public string Agent { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "開始日期")]
        public DateTime? StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "結束日期")]
        public DateTime? EndDate { get; set; }
        [Required]
        [Display(Name = "開始時間")]
        [Time]
        public int? StartTime { get; set; }
        [Required]
        [Display(Name = "結束時間")]
        [Time]
        public int? EndTime { get; set; }
        [Range(0, 8)]
        public int? Hour 
        {
            get
            {
                int? h = EndTime - StartTime;
                return h / 8 == 1 ? 0 : h;
            }
        }
        [Range(0, 22)]
        public double? Days
        {
            get
            {
                double h = (EndTime - StartTime) / 8 == 1 ? 1 : 0;
                return CalculateWorkingDays(StartDate, EndDate, new List<string>()) + h;
            }
        }
        public List<SelectListItem> SelectTime
        {
            get
            {
                return new List<SelectListItem>() { 
                    new SelectListItem() { Value = "1", Text = "08:00" },
                    new SelectListItem() { Value = "2", Text = "09:00" },
                    new SelectListItem() { Value = "3", Text = "10:00" },
                    new SelectListItem() { Value = "4", Text = "11:00" },
                    new SelectListItem() { Value = "5", Text = "12:00" },
                    new SelectListItem() { Value = "6", Text = "13:50" },
                    new SelectListItem() { Value = "7", Text = "14:50" },
                    new SelectListItem() { Value = "8", Text = "15:50" },
                    new SelectListItem() { Value = "9", Text = "16:50" },
                };
            }
        }

        public double CalculateWorkingDays(DateTime? StartDate, DateTime? EndDate, List<string> Holidays)
        {
            if (StartDate == null && EndDate == null)
                throw new Exception("StartDate or EndDate is null");
            double days = 0;
            DateTime startdate = (DateTime)StartDate;
            DateTime enddate = (DateTime)EndDate;
            while (startdate < enddate)
            {
                startdate = startdate.AddDays(1);
                if (startdate.DayOfWeek == DayOfWeek.Saturday || startdate.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                if (Holidays.Where(x => x.Contains($"{startdate:yyyy/MM/dd}")).Count() > 0)
                    continue;
                days++;
            }
            return days;
        }
    }

}
