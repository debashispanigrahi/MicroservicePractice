import { Observable } from 'rxjs';
import { Employee } from '../models/employee';
import { AddEmployeeInput } from '../models/add-employee-input';

export abstract class EmployeeDataService {

  abstract getAll(
    fields?: string[]
  ): Observable<Employee[]>;

  abstract getById(
    id: number,
    fields?: string[]
  ): Observable<Employee | null>;

  abstract add(
    input: AddEmployeeInput
  ): Observable<Employee>;

  abstract upload(
    file: File
  ): Observable<number>;
}