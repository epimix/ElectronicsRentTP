namespace ElectronicsRentTP.Models
{
    public enum DialogType
    {
        danger,
        info,
        success,
        warning
    }
    public class ConfirmDialogModel
    {
        public string DialogId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Message { get; set; }
        // danger, warning, info, success
        public DialogType Mode { get; set; } = DialogType.info;
    }
}

