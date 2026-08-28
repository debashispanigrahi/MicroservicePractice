import { gql } from 'apollo-angular';

export const LOGIN_MUTATION = gql`
  mutation Login($input: AuthCredsInput!) {
    login(input: $input) {
      accessToken
      expiresAtUtc
    }
  }
`;