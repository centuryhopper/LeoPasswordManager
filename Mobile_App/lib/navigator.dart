import 'package:flutter/material.dart';
import 'package:mobile_app/Models/LoginDTO.dart';
import 'package:mobile_app/constants.dart';
import 'package:mobile_app/main.dart';
import 'package:mobile_app/pages/loginpage.dart';
import 'package:mobile_app/pages/passwordspage.dart';
import 'package:mobile_app/pages/profilepage.dart';
import 'package:mobile_app/pages/settingspage.dart';

LoginDTO? _loginDTO;

// this helper widget will help avoid circular dependencies among the other dart files
class NavigationHelperWidget extends StatefulWidget {
  static const String routeID = "/helper";

  NavigationHelperWidget(LoginDTO? loginDTO, {super.key}) {
    _loginDTO = loginDTO;
  }

  @override
  _NavigationHelperWidgetState createState() => _NavigationHelperWidgetState();
}

class _NavigationHelperWidgetState extends State<NavigationHelperWidget> {
  int _selectedIndex = 0;

  final List<Widget> pages = [
    PasswordsPage(loginDTO: _loginDTO),
    SettingsPage(loginDTO: _loginDTO),
    ProfilePage(loginDTO: _loginDTO),
  ];

  @override
  void initState() {
    super.initState();
    // signIn();
  }

  @override
  void dispose() {
    super.dispose();
  }

  // Logout method
  void _logout() {
    // Clear any user data or tokens here, for example:
    // await secureStorage.delete(key: 'jwtToken');

    // Navigate back to login screen and remove all previous routes
    Navigator.of(context).pushAndRemoveUntil(
      MaterialPageRoute(builder: (context) => AppHome()),
      (Route<dynamic> route) => false,
    );
  }

  // navigate to the page based on the index selected
  void _onItemTapped(int bottomNavButtonIndex) {
    _selectedIndex = bottomNavButtonIndex;
    setState(() {});

    if (bottomNavButtonIndex == 3) {
      _logout();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: pages[_selectedIndex],

      // https://api.flutter.dev/flutter/material/BottomNavigationBar-class.html
      // https://www.youtube.com/watch?v=elLkVWt7gRM&ab_channel=ProgrammingAddict
      bottomNavigationBar: BottomNavigationBar(
          items: ourBottomNavBar(),
          currentIndex: _selectedIndex,
          selectedItemColor: Colors.black,
          onTap: _onItemTapped),
    );
  }
}
