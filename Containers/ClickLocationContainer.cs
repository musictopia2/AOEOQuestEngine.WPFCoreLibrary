namespace AOEOQuestEngine.WPFCoreLibrary.Containers;
public class ClickLocationContainer
{
    // Make Locations mutable at first but read-only afterward
    private BasicList<Point> _locations = [];
    public IReadOnlyList<Point> Locations => _locations.AsReadOnly();
    // Flag to track if it has been populated
    private bool _isPopulated = false;
    // This method allows setting Locations after injection
    public void PopulateLocations(BasicList<Point> locations)
    {
        if (_isPopulated)
        {
            return; //already populated.
            //throw new InvalidOperationException("Locations have already been populated and cannot be changed.");
        }
        _locations = locations;
        _isPopulated = true;  // Mark as populated
    }
}