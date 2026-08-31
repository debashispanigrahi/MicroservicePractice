import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { Employee } from '../../../core/models/employee';
import { EmployeeDataService } from '../../../core/services/employee-data.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, MatIconModule, MatButtonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  private readonly employeeService = inject(EmployeeDataService);

  private readonly cdr = inject(ChangeDetectorRef);

  employees: Employee[] = [];

  filteredEmployees: Employee[] = [];

  selectedEmployee: Employee | null = null;

  isLoading = false;

  errorMessage = '';

  searchTerm = '';

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    console.log('Loading employees...');

    this.isLoading = true;
    this.errorMessage = '';

    this.employeeService.getAll().subscribe({
      next: (employees) => {
        console.log('Dashboard received:', employees);

        this.employees = employees ?? [];

        this.filteredEmployees = [...this.employees];

        this.isLoading = false;

        // Force Angular to update the view
        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error('Dashboard GraphQL error:', error);

        this.errorMessage = 'Unable to load employees. Please try again.';

        this.isLoading = false;

        this.cdr.detectChanges();
      },

      complete: () => {
        console.log('Employee request completed.');
      },
    });
  }

  searchEmployees(): void {
    const term = this.searchTerm.trim().toLowerCase();

    if (!term) {
      this.filteredEmployees = [...this.employees];

      return;
    }

    this.filteredEmployees = this.employees.filter(
      (employee) =>
        employee.name.toLowerCase().includes(term) ||
        employee.email.toLowerCase().includes(term) ||
        employee.employeeId.toString().includes(term),
    );
  }

  selectEmployee(employee: Employee): void {
    this.selectedEmployee = employee;
  }

  clearSelection(): void {
    this.selectedEmployee = null;
  }

  get totalEmployees(): number {
    return this.employees.length;
  }

  get visibleEmployees(): number {
    return this.filteredEmployees.length;
  }
}
