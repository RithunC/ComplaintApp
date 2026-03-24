import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [AppComponent, RouterTestingModule, HttpClientTestingModule] }).compileComponents();
  });
  it('should create', () => { const f = TestBed.createComponent(AppComponent); expect(f.componentInstance).toBeTruthy(); });
});
