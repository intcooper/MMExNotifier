# MMExNotifier

This Windows desktop application shows the upcoming occurrences of the recurring transactions defined in a [MoneyManagerEx](https://github.com/moneymanagerex/moneymanagerex) database.

The idea comes from the way I use MoneyManagerEx: I mainly open it once or twice a month to insert all the expenses and reconcile the accounts. As I also schedule all the recurring expenses in MMEx, I wanted to find a way to remind myself of the upcoming ones without necessarily opening the application.

MMExNotifier opens the specified MoneyManagerEx database and compiles a list of the upcoming recurring transactions. If any, a notification is shown: a click on the notification results in showing the main window containing the details of the occurrences.

## Dependencies

MMExNotifier makes use of the following libraries:

- [Microsoft.Toolkit.Uwp.Notifications](https://www.nuget.org/packages/Microsoft.Toolkit.Uwp.Notifications) to show the notifications.
- [System.Data.SQLite](https://www.nuget.org/packages/System.Data.SQLite) to access the MMEx database.

## Compatibility

MMExNotifier has been tested on Windows 10.

## Known Issues

Encrypted MMEx databases are not supported.
