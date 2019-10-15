using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;

public class MyColumnHeader : ColumnHeader
{
    [DefaultValue(typeof(Control), null)]
    public Control Control { get; set; }

    [DefaultValue("")]
    public string SelectValues { get; set; } = "";

    public string GetTextFromDispatching()
    {
        // if no control is linked to the column return Nothing
        string text = null;

        if (Control != null)
        {
            if (Control.GetType().GetProperty("CheckState") != null)
            {
                text = this.GetCheckValue();
            }
            else
            {
                text = Control.Text;
            }
        }

        return text;
    }

    /*  
    * returns "Y" or "N" depending on whether a bound checkbox is checked or not
    * if CheckBox has three check states (ThreeState = true) return "N/A"   
    */
    public string GetCheckValue()
    {
        string checkValue = "";
        if (Control != null && Control.GetType().GetProperty("CheckState") != null)
        {
            CheckState StateValue;
            StateValue = (CheckState)Control.GetType().GetProperty("CheckState").GetValue(Control);
            var valeurs = SelectValues.Split('|');
            if (StateValue == CheckState.Checked)
            {
                if (!string.IsNullOrEmpty(SelectValues))
                    checkValue = valeurs.Length > 0 ? valeurs[0] : "";
                else
                    checkValue = "Y";
            }
            else if (StateValue == CheckState.Unchecked)
            {
                if (!string.IsNullOrEmpty(SelectValues))
                    checkValue = valeurs.Length > 1 ? valeurs[1] : "";
                else
                    checkValue = "N";
            }
            else if (StateValue == CheckState.Indeterminate)
            {
                if (!string.IsNullOrEmpty(SelectValues))
                    checkValue = valeurs.Length > 2 ? valeurs[2] : "";
                else
                    checkValue = "N/A";
            }
        }

        return checkValue;
    }

    /* updates the checkbox checkstate according to the value contained in the selected ListView row
    * "Y", "Yes", "T", "True" will "check" the bound checkbox
    * "N", "No", "F", "False" will "uncheck" the checkbox
    * other values will produce an "indeterminate" state 
    */
    public CheckState GetCheckState(string valueChecked)
    {
        CheckState checkState;
        string[] vals = { "Y", "Yes", "T", "True" };

        if (!string.IsNullOrEmpty(SelectValues))
        {
            var valeurs = SelectValues.Split('|');
            if (valueChecked.Equals(valeurs.Length > 0 ? valeurs[0] : ""))
                checkState = CheckState.Checked;
            else if (valueChecked.Equals(valeurs.Length > 1 ? valeurs[1] : ""))
                checkState = CheckState.Unchecked;
            else
                checkState = CheckState.Indeterminate;
        }
        else if (vals.Any(x => x.Contains(valueChecked)))
            checkState = CheckState.Checked;
        else if (new[] { "N", "No", "F", "False" }.Contains(valueChecked))
            checkState = CheckState.Unchecked;
        else
            checkState = CheckState.Indeterminate;
        return checkState;
    }
}
