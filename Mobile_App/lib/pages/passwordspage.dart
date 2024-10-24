import 'package:PasswordManager/Models/PasswordAccountDTO.dart';
import 'package:PasswordManager/Services/password_manager_service.dart';
import 'package:PasswordManager/passwordtable.dart';
import 'package:PasswordManager/statemanagement/bloc/PasswordVisibility/password_visibility_bloc.dart';
import 'package:PasswordManager/statemanagement/bloc/PasswordVisibility/password_visibility_event.dart';
import 'package:PasswordManager/statemanagement/bloc/PasswordVisibility/password_visibility_state.dart';
import 'package:flutter/material.dart';
import 'package:PasswordManager/Models/LoginDTO.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

// TODO: show passwords in a grid with futurebuilder and toggle showing the passwords using bloc state management

class PasswordsPage extends StatelessWidget {
  const PasswordsPage({
    super.key,
    LoginDTO? loginDTO,
  });

  // Simulating an API call to fetch password accounts
  Future<List<PasswordAccountDTO>?> fetchPasswordAccounts() async {
    var passwordAccounts = await PasswordManagerService.getPasswordAccounts();
    if (passwordAccounts == null) {
      throw Exception("Couldn't retrieve password accounts from the server");
    }
    return passwordAccounts;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Password Manager'),
        automaticallyImplyLeading: false,
      ),
      body: FutureBuilder<List<PasswordAccountDTO>?>(
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
            return SingleChildScrollView(
              child: Column(
                children: [
                  BlocBuilder<PasswordVisibilityBloc, PasswordVisibilityState>(
                    builder: (ctx, state) => PaginatedDataTable(
                      header: const Text('Password Accounts'),
                      columns: const [
                        DataColumn(label: Text('ID')),
                        DataColumn(label: Text('Title')),
                        DataColumn(label: Text('Username')),
                        DataColumn(label: Text('Password')),
                        DataColumn(label: Text('Created At')),
                        DataColumn(label: Text('Last Updated At')),
                      ],
                      source: PasswordTableSource(
                          passwordAccounts,
                          ctx.read<
                              PasswordVisibilityBloc>()), // Custom data source
                      rowsPerPage: 10, // Set the number of rows per page
                    ),
                  ),
                  const SizedBox(
                    height: 30.0,
                  ),
                  ElevatedButton(
                      onPressed: () {
                        BlocProvider.of<PasswordVisibilityBloc>(context)
                            .add(PasswordVisibilityToggle());
                      },
                      child: const Text('Show/Hide Password')),
                  const SizedBox(
                    height: 30.0,
                  ),
                ],
              ),
            );
          }
        },
      ),
    );
  }
}
