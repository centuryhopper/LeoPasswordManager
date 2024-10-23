import 'package:PasswordManager/Models/PasswordAccountDTO.dart';
import 'package:PasswordManager/passwordtable.dart';
import 'package:flutter/material.dart';
import 'package:PasswordManager/Models/LoginDTO.dart';

// TODO: show passwords in a grid with futurebuilder and toggle showing the passwords using bloc state management

class PasswordsPage extends StatelessWidget {
  const PasswordsPage({
    super.key,
    LoginDTO? loginDTO,
  });

  // Simulating an API call to fetch password accounts
  Future<List<PasswordAccountDTO>> fetchPasswordAccounts() async {
    await Future.delayed(const Duration(seconds: 2)); // Simulate network delay

    // Dummy data
    return List.generate(
      100,
      (index) => PasswordAccountDTO(
        id: index + 1,
        userId: 1,
        title: "Account $index",
        username: "user$index",
        password: "password$index",
        createdAt: DateTime.now().subtract(Duration(days: index)),
        lastUpdatedAt: DateTime.now(),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Password Manager'),
      ),
      body: FutureBuilder<List<PasswordAccountDTO>>(
        future: fetchPasswordAccounts(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(child: Text('Error: ${snapshot.error}'));
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(child: Text('No password accounts available'));
          } else {
            final passwordAccounts = snapshot.data!;
            return PaginatedDataTable(
              header: const Text('Password Accounts'),
              columns: const [
                DataColumn(label: Text('ID')),
                DataColumn(label: Text('Title')),
                DataColumn(label: Text('Username')),
                DataColumn(label: Text('Password')),
                DataColumn(label: Text('Created At')),
                DataColumn(label: Text('Last Updated At')),
              ],
              source: PasswordTableSource(passwordAccounts), // Custom data source
              rowsPerPage: 10, // Set the number of rows per page
            );
          }
        },
      ),
    );
  }
}

