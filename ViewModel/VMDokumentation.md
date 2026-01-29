# ViewModels  

## BaseViewModel  

Diese Klasse sorgt dafür, dass Änderungen automatisch an die UI weitergeben werden.  

### Attribute  

public event PropertyChangedEventHandler PropertyChanged    

### Methoden  

public void OnPropertyChanged(string propertyName)  

- Erzeugt ein PropertyChangedEventArgs-Objekt mit dem Namen der geänderten Property.  
- Ruft die PropertyChanged-Methode auf, um alle Abonnenten zu benachrichtigen.  

## HomeViewModel

### Attribute


