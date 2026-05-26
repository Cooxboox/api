# TODO

- (✅) Un utilisateur doté d’un mot de passe peut quand même effectuer une connexion sans mot de passe, à condition que son `MultiFactorAuthenticationMode != Email`.
- (✅) Le Front-End peut demander une connexion sans mot de passe pour un utilisateur doté d’un mot de passe (booléen ajouté à `Credentials`).
- ( ) Le Back-End doit informer le Front-End si la connexion sans mot de passe est possible ou non (`IsPasswordRequired` devient autre chose qu’un booléen).
- (✅) Si `MultiFactorAuthenticationMode == Email`, l’utilisateur doit absolument posséder un mot de passe.
- (✅) Un utilisateur sans mot de passe peut avoir un `MultiFactorAuthenticationMode == Phone`.
- (✅) Au retour d’un jeton d’authentification, si `MultiFactorAuthenticationMode == Phone`, effectuer une authentification par message texte.
