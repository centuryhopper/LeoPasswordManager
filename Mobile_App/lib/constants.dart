import 'package:flutter/material.dart';

List<BottomNavigationBarItem> ourBottomNavBar() {
  return const <BottomNavigationBarItem>[
    BottomNavigationBarItem(
        backgroundColor: Colors.red,
        icon: Icon(Icons.add, color: Colors.black),
        label: 'Passwords'),
    BottomNavigationBarItem(
        backgroundColor: Colors.green,
        icon: Icon(Icons.chat, color: Colors.black),
        label: 'Settings'),
    BottomNavigationBarItem(
        backgroundColor: Colors.purple,
        icon: Icon(Icons.search, color: Colors.black),
        label: 'Profile'),
    BottomNavigationBarItem(
      backgroundColor: Colors.blue,
      icon: Icon(Icons.logout, color: Colors.black),
      label: 'Logout'),
  ];
}