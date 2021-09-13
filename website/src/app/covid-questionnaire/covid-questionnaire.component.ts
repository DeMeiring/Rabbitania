import { Component, OnInit, ɵNOT_FOUND_CHECK_ONLY_ELEMENT_INJECTOR } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { QuestionnaireRequest} from '../interfaces/covid_questionnaire_interface';
import { QuestionnaireResponse} from '../interfaces/covid_questionnaire_interface';
import { AiServiceService } from '../../app/services/AI/ai-service.service';
import { UserDetailsService } from '../services/user-details/user-details.service';
import { SignOutComponent } from '../sign-out/sign-out.component';
import { AuthService } from '../services/firebase/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-covid-questionnaire',
  templateUrl: './covid-questionnaire.component.html',
  styleUrls: ['./covid-questionnaire.component.scss']
})
export class CovidQuestionnaireComponent implements OnInit{
user_email = "";

  constructor(
    private fb: FormBuilder,
    private service : AiServiceService,
    private auth_service: AuthService,
    private userService: UserDetailsService,
    private router : Router) {

  }

  async ngOnInit(){

}


  covidQuestionnaire = this.fb.group({
    cough: false,
    fever: false,
    sore_throat: false,
    shortness_of_breath: false,
    head_ache: false,
    test_indication: false,
    gender: "male",
    });


  checkOnline(){
    if(this.user_email == "")
    {
      return false;
    }
    else return true;
  }

  async onSubmit(){
    console.log(this.checkOnline());
    if(this.checkOnline() == false){
      var res = await this.auth_service.signIn();
      if(res){
        this.user_email = this.userService.retrieveUserDetails().email;
        const questionnaireObject = this.covidQuestionnaire.value;

        var _cough = "0";
        var _fever = "0";
        var _sore_throat = "0";
        var _shortness_of_breath = "0";
        var _head_ache = "0";
        var _test_indication = "Other";
        var _gender = "male";

          if(questionnaireObject.cough == true){
            _cough = "1";
          }
          if(questionnaireObject.fever == true){
            _fever = "1";
          }
          if(questionnaireObject.sore_throat == true){
            _sore_throat = "1";
          }
          if(questionnaireObject.shortness_of_breath == true){
            _shortness_of_breath = "1";
          }
          if(questionnaireObject.head_ache == true){
            _head_ache = "1";
          }
          if(questionnaireObject.gender == "female"){
            _gender = "female";
          }
          if(questionnaireObject.test_indication == true){
            _test_indication = "Contact with confirmed"
          }

          var result = (await this.service.Post(_cough, _fever, _sore_throat, _shortness_of_breath, _head_ache, _gender, _test_indication))

          result.subscribe(async data => {
            if(data){
              console.log(data);
            }
          });
          var activateResult = (await (this.service.Activate(this.user_email))).subscribe(async data => {
            if(data){
              console.log(data);
            }
          });

        }
      }
      else{
        this.user_email = this.userService.retrieveUserDetails().email;
        const questionnaireObject = this.covidQuestionnaire.value;

        var _cough = "0";
        var _fever = "0";
        var _sore_throat = "0";
        var _shortness_of_breath = "0";
        var _head_ache = "0";
        var _test_indication = "Other";
        var _gender = "male";

          if(questionnaireObject.cough == true){
            _cough = "1";
          }
          if(questionnaireObject.fever == true){
            _fever = "1";
          }
          if(questionnaireObject.sore_throat == true){
            _sore_throat = "1";
          }
          if(questionnaireObject.shortness_of_breath == true){
            _shortness_of_breath = "1";
          }
          if(questionnaireObject.head_ache == true){
            _head_ache = "1";
          }
          if(questionnaireObject.gender == "female"){
            _gender = "female";
          }
          if(questionnaireObject.test_indication == true){
            _test_indication = "Contact with confirmed"
          }

          var result = (await this.service.Post(_cough, _fever, _sore_throat, _shortness_of_breath, _head_ache, _gender, _test_indication))

          result.subscribe(async data => {
            if(data){
              console.log(data);
            }
          });
          var activateResult = (await (this.service.Activate(this.user_email))).subscribe(async data => {
            if(data){
              console.log(data);
            }
          });



      }
      this.router.navigate(['/'], {
        queryParams: {

        },
        skipLocationChange: false
      });
    }

}



