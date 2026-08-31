import { Injectable, inject } from '@angular/core';
import { Apollo } from 'apollo-angular';
import { gql } from '@apollo/client/core';

import { Observable, map } from 'rxjs';

import { EmployeeDataService } from './employee-data.service';
import { Employee } from '../models/employee';
import { AddEmployeeInput } from '../models/add-employee-input';

@Injectable({
  providedIn: 'root',
})
export class GraphQLEmployeeService extends EmployeeDataService {
  private readonly apollo = inject(Apollo);

  override getAll(fields?: string[]): Observable<Employee[]> {
    const query = gql`
      query GetEmployees {
        employees {
          employeeId
          name
          email
          createdAt
        }
      }
    `;

    return this.apollo
      .query<{
        employees: Employee[];
      }>({
        query,
        fetchPolicy: 'network-only',
      })
      .pipe(
        map((result) => {
          return result.data?.employees ?? [];
        }),
      );
  }

  override getById(id: number, fields?: string[]): Observable<Employee | null> {
    const query = gql`
      query GetEmployee($id: Int!) {
        employee(id: $id) {
          employeeId
          name
          email
          createdAt
        }
      }
    `;

    return this.apollo
      .query<{
        employee: Employee | null;
      }>({
        query,
        variables: {
          id,
        },
        fetchPolicy: 'network-only',
      })
      .pipe(map((result) => result.data?.employee ?? null));
  }

  override add(input: AddEmployeeInput): Observable<Employee> {
    const mutation = gql`
      mutation AddEmployee($input: AddEmployeeInput!) {
        addEmployee(input: $input) {
          name
          email
        }
      }
    `;

    return this.apollo
      .mutate<{
        addEmployee: Employee;
      }>({
        mutation,
        variables: {
          input,
        },
      })
      .pipe(
        map((result) => {
          if (!result.data?.addEmployee) {
            throw new Error('Employee could not be added.');
          }

          return result.data.addEmployee;
        }),
      );
  }

  override upload(file: File): Observable<number> {
    throw new Error('Not implemented yet.');
  }
}
