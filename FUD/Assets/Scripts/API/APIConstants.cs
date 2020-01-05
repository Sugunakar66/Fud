﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIConstants 
{
    public const string SECRET_KEY = "SSht6KyxvwKZnsmbbbQUCm2va";

    public const string BASE_URL = "http://3.136.106.58:7000/";

    public static string TOKEN_PATH = Application.persistentDataPath + "/Token";

    public const string CREATE_USER = BASE_URL + "v1/users/";

    public const string USER_LOGIN = BASE_URL + "v1/login/";

    public const string USER_OTP = BASE_URL + "v1/sendOtp";

    public const string GET_CRATFS = BASE_URL + "v1/24_craftRoles/";

    public const string GET_GENRES = BASE_URL + "v1/genres/";

    public const string CREATE_STORY = BASE_URL + "v1/story/";

    public const string CREATE_PROJECT = BASE_URL + "v1/projects/";
}