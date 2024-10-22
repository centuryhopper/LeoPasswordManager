import 'package:flutter/material.dart';
import 'package:PasswordManager/Models/LoginDTO.dart';

// TODO: show passwords in a grid with futurebuilder and toggle showing the passwords using bloc state management

class PasswordsPage extends StatelessWidget {
  const PasswordsPage({
    super.key,
    LoginDTO? loginDTO,
  });


  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.all(20),
      padding: const EdgeInsets.all(16),
      child: const Center(
        child: Text("This is the password page haha"),
      ),
    );
  }
}
