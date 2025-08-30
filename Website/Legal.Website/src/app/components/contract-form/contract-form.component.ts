import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ContractService } from '../../services/contract.service';
import { AuthService } from '../../services/auth.service';
import { Contract } from '../../models/contract.model';

@Component({
  selector: 'app-contract-form',
  templateUrl: './contract-form.component.html',
  styleUrls: ['./contract-form.component.css'],
  standalone: false
})
export class ContractFormComponent implements OnInit {
  contractForm!: FormGroup;
  loading = false;
  error = '';
  isEditMode = false;
  contractId?: string;

  constructor(
    private formBuilder: FormBuilder,
    private contractService: ContractService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.checkEditMode();
  }

  private initializeForm(): void {
    this.contractForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      author: ['', [Validators.required, Validators.minLength(2)]],
      description: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  private checkEditMode(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.contractId = params['id'];
        // Add null check before calling loadContract
        if (this.contractId) {
          this.loadContract(this.contractId);
        }
      }
    });
  }

  private loadContract(id: string): void {
    this.loading = true;
    this.contractService.getContract(id).subscribe({
      next: (contract: Contract) => {
        this.contractForm.patchValue({
          name: contract.name,
          author: contract.author,
          description: contract.description
        });
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load contract';
        this.loading = false;
        console.error('Error loading contract:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.contractForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.loading = true;
    this.error = '';

    const formValue = this.contractForm.value;
    const contract: Contract = {
      id: this.contractId || this.generateId(),
      name: formValue.name,
      author: formValue.author,
      description: formValue.description,
      created: this.isEditMode ? new Date() : new Date(), // This will be set by backend
      updated: this.isEditMode ? new Date() : undefined
    };

    this.contractService.saveContract(contract).subscribe({
      next: (savedContract) => {
        this.router.navigate(['/contracts']);
      },
      error: (error) => {
        this.error = this.isEditMode ? 'Failed to update contract' : 'Failed to create contract';
        this.loading = false;
        console.error('Error saving contract:', error);
      }
    });
  }

  private generateId(): string {
    return 'contract-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
  }

  private markFormGroupTouched(): void {
    Object.keys(this.contractForm.controls).forEach(key => {
      const control = this.contractForm.get(key);
      control?.markAsTouched();
    });
  }

  cancel(): void {
    this.router.navigate(['/contracts']);
  }

  getCurrentUser() {
    return this.authService.getCurrentUser();
  }

  logout(): void {
    this.authService.logout();
  }
}
