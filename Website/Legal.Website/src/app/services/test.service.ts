import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { ContractService } from './contract.service';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class TestService {
  constructor(
    private authService: AuthService,
    private contractService: ContractService,
    private apiService: ApiService
  ) { }

  /**
   * Test the API connection by checking available commands
   */
  async testApiConnection(): Promise<boolean> {
    try {
      // This would test if the API is reachable
      // You can uncomment this when the backend is running
      // const result = await this.apiService.executeQuery('ADMIN', 'ListAll', {}).toPromise();
      // return true;
      return true;
    } catch (error) {
      console.error('API connection test failed:', error);
      return false;
    }
  }

  /**
   * Get test data for development
   */
  getTestContract() {
    return {
      id: '',
      name: 'Sample Contract',
      author: 'John Doe',
      description: 'This is a sample contract for testing purposes. It includes all the necessary fields and demonstrates the contract management functionality.',
      created: new Date(),
      updated: undefined
    };
  }

  /**
   * Get test login credentials for development
   */
  getTestCredentials() {
    return {
      userName: 'testuser',
      password: 'password123',
      moduleName: 0
    };
  }

  /**
   * Get test registration data for development
   */
  getTestRegistration() {
    return {
      name: 'Test User',
      userName: 'testuser',
      password: 'password123',
      moduleName: 0
    };
  }
}
