import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { finalize } from 'rxjs';

import { ToastrService } from '@iqx-limited/ngx-toastr';

import { EmployeeDataService } from '../../../core/services/employee-data.service';
import { AddEmployeeInput } from '../../../core/models/add-employee-input';

@Component({
  selector: 'app-add',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './add.html',
  styleUrl: './add.scss',
})
export class Add {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly employeeDataService = inject(EmployeeDataService);
  private readonly toastr = inject(ToastrService);

  loading = false;

  addEmployeeForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
  });

  submit(): void {
    if (this.addEmployeeForm.invalid || this.loading) {
      this.addEmployeeForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    const input: AddEmployeeInput = {
      name: this.addEmployeeForm.controls.name.value.trim(),
      email: this.addEmployeeForm.controls.email.value.trim(),
    };

    try {
      this.employeeDataService
        .add(input)
        .pipe(
          finalize(() => {
            this.loading = false;
          }),
        )
        .subscribe({
          next: () => {
            this.toastr.success('Employee added successfully.', 'Success');

            this.router.navigate(['/employees']);
          },

          error: (error) => {
            console.error('Add employee failed:', error);

            this.toastr.error(this.getErrorMessage(error), 'Unable to add employee');
          },
        });
    } catch (error) {
      this.loading = false;
      console.error('Add employee failed:', error);

      this.toastr.error(this.getErrorMessage(error), 'Unable to add employee');
    }
  }

  cancel(): void {
    this.router.navigate(['/employees']);
  }

  private getErrorMessage(error: unknown): string {
    if (error && typeof error === 'object' && 'message' in error) {
      return String((error as { message: unknown }).message);
    }

    return 'Something went wrong while adding the employee.';
  }
}
