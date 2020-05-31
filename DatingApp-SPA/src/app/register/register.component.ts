import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();

  model: any = {};

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {}

  register() {

    const x = this.model;

    this.authService.register(this.model).subscribe(
      () => {
        this.alertify.success('registration successfull');
      },
      (error) => {
        if(!x.username){
          this.alertify.error("Username is required");
        }
        if(!x.password){
          this.alertify.error("Password is required");
        }
      }
    );
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
