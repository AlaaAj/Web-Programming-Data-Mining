using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HeartDisease.Models
{
    public class Id3
    {
		


		public value classification(value PATIENT_VALUE)
        {


			if (PATIENT_VALUE.chest_pain_type == "atyp_angina" || PATIENT_VALUE.chest_pain_type == "non_anginal")
			{
				if (PATIENT_VALUE.exercice_angina == "no")
                {
					PATIENT_VALUE.result = "false";
					return PATIENT_VALUE;
                }
				else
				{
					if (PATIENT_VALUE.max_heart_rate <= 151)
					{
						if (PATIENT_VALUE.max_heart_rate <= 140.500)
                        {
							PATIENT_VALUE.result = "false";
							return PATIENT_VALUE;
						}
							
						else
                        {
							PATIENT_VALUE.result = "true";
							return PATIENT_VALUE;
						}
							
					}
					else
                    {
						PATIENT_VALUE.result = "false";
						return PATIENT_VALUE;
					}
						

				}
			}

			else
			{
				if (PATIENT_VALUE.exercice_angina == "no")
				{
					if (PATIENT_VALUE.max_heart_rate <= 131)
                    {
						PATIENT_VALUE.result = "true";
						return PATIENT_VALUE;
					}
					else
					{
						if (PATIENT_VALUE.max_heart_rate <= 149)
                        {
							PATIENT_VALUE.result = "false";
							return PATIENT_VALUE;
						}
						
						else
                        {
							PATIENT_VALUE.result = "true";
							return PATIENT_VALUE;
						}
							
					}
				}
				else if (PATIENT_VALUE.exercice_angina == "yes")
				{
					PATIENT_VALUE.result = "true";
					return PATIENT_VALUE;
				}
				
			}



			return PATIENT_VALUE;
        }
    }
}