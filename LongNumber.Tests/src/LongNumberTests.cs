using System;
using Xunit;

namespace LongNumber.Tests
{
    public class LongNumberTests
    {
        [Theory]
        [MemberData(nameof(ConvertToLongFormData))]
        public void Validate_ConvertToLongForm(string numericString, string expectedResult)
        {
            var result = LongNumber.ConvertToLongForm(numericString);
            Assert.Equal(expectedResult, result);
        }

        public static TheoryData<string, string> ConvertToLongFormData => new TheoryData<string, string>
        {
            { "1000000000001", "One trillion one"                        },
            {         "30542", "Thirty thousand five hundred fourty-two" },
            {          "5000", "Five thousand"                           },
            {           "123", "One hundred twenty-three"                },
            {           "100", "One hundred"                             },
            {            "43", "Fourty-three"                            },
            {            "20", "Twenty"                                  },
            {            "19", "Nineteen"                                },
            {             "0", "Zero"                                    }
        };

        [Theory]
        [MemberData(nameof(ConvertCurrencyToLongFormData))]
        public void Validate_ConvertCurrencyToLongForm(string currencyString, string expectedResult)
        {
            var result = LongNumber.ConvertCurrencyToLongForm(currencyString);
            Assert.Equal(expectedResult, result);
        }

        public static TheoryData<string, string> ConvertCurrencyToLongFormData => new TheoryData<string, string>
        {
            { "-$5,000,032.01", "Negative five million thirty-two dollars and one cent"     },
            {      "$1,337.00", "One thousand three hundred thirty-seven dollars"           },
            {        "5612.20", "Five thousand six hundred twelve dollars and twenty cents" },
            {          "$0.15", "Fifteen cents"                                             },
            {             "45", "Fourty-five dollars"                                       },
            {          "$0001", "One dollar"                                                },
            {             "$0", "Zero dollars"                                              }
        };

        [Theory]
        [MemberData(nameof(ConvertCurrencyToLongFormInvalidData))]
        public void Validate_ConvertCurrencyToLongForm_InvalidCurrencyString(string invalidCurrencyString)
        {
            var exception = Assert.Throws<ArgumentException>(() => LongNumber.ConvertCurrencyToLongForm(invalidCurrencyString));
            Assert.Equal("currencyString", exception.ParamName);
        }

        public static TheoryData<string> ConvertCurrencyToLongFormInvalidData => new TheoryData<string>
        {
            {     "$134.504" }, // More than two decimal places in fractional amount
            {         "$54." }, // Fractional amount with no decimal places
            {          "0.6" }, // Fractional amount with one decimal place
            {       "+$0.43" }, // Positive sign before dollar sign (only allow negative sign in this position)
            {         "$.60" }, // Fractional amount provided without dollar amount
            { "$,432,352.00" }, // Commas are provided, but dollar amount must begin with a 1-3 digit sequence
            {   "$13,56,132" }  // Commas are provided, but all sequences after the first comma must be three digits long
        };
    }
}
