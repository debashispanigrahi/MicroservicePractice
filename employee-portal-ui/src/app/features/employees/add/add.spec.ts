import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';

import { ToastrService } from '@iqx-limited/ngx-toastr';

import { Add } from './add';
import { EmployeeDataService } from '../../../core/services/employee-data.service';

describe('Add', () => {
  const navigateSpy = vi.fn();

  const toastrServiceStub = {
    success: vi.fn(),
    error: vi.fn(),
  };

  const employeeDataServiceStub = {
    getAll: vi.fn().mockReturnValue(of([])),
    getById: vi.fn().mockReturnValue(of(null)),
    add: vi.fn(() => {
      throw new Error('Synchronous add failure');
    }),
    upload: vi.fn().mockReturnValue(of(0)),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Add],
      providers: [
        { provide: Router, useValue: { navigate: navigateSpy } },
        { provide: ToastrService, useValue: toastrServiceStub },
        { provide: EmployeeDataService, useValue: employeeDataServiceStub },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should reset the loading state when add throws synchronously', () => {
    const fixture = TestBed.createComponent(Add);
    const component = fixture.componentInstance;

    component.addEmployeeForm.setValue({
      name: 'Jane Doe',
      email: 'jane@example.com',
    });

    expect(() => component.submit()).not.toThrow();
    expect(component.loading).toBeFalsy();
    expect(toastrServiceStub.error).toHaveBeenCalled();
  });
});
