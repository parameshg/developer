terraform {
  backend "s3" {
    bucket = "aspnetcore"
    key    = "terraform-state/developer-api.tfstate"
  }
}

variable "AWS_ACCOUNT" {
  type = string
  description = "AWS_ACCOUNT"
}

variable "AWS_REGION" {
  type = string
  description = "AWS_REGION"
}

variable "AWS_ACCESS_KEY_ID" {
  type = string
  description = "AWS_ACCESS_KEY_ID"
}

variable "AWS_SECRET_ACCESS_KEY" {
  type = string
  description = "AWS_SECRET_ACCESS_KEY"
}

variable "IMAGE_COMMAND" {
  type        = list(string)
  description = "Docker Image CMD Override"
  default     = ["Developer.Api::Developer.Api.LambdaEntryPoint::FunctionHandlerAsync"]
}

variable "SENTRY_ENDPOINT" {
  type    = string
  description = "Sentry Dsn"
  default = ""
}

variable "HTTP_TIMEOUT" {
  type        = number
  description = "Lambda Timeout"
  default     = 10
}

provider "aws" {
  region     = var.AWS_REGION
  access_key = var.AWS_ACCESS_KEY_ID
  secret_key = var.AWS_SECRET_ACCESS_KEY
}

# AWS IAM ROLE ####################################################################################################################################################

data "aws_iam_policy_document" "developer-api-trust" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }
  }
}

data "aws_iam_policy_document" "developer-api-policy" {
  statement {
    actions   = ["dynamodb:*"]
    resources = ["arn:aws:dynamodb:${var.AWS_REGION}:${var.AWS_ACCOUNT}:table/*"]
  }
}

resource "aws_iam_role" "developer-api" {
  name               = "developer-api"
  managed_policy_arns = ["arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"]
  assume_role_policy = data.aws_iam_policy_document.developer-api-trust.json
  inline_policy {
    name   = "inline-policy"
    policy = data.aws_iam_policy_document.developer-api-policy.json
  }
  tags               = {
    provisioner      = "terraform"
    executioner      = "github-actions"
    project          = "developer-api"
    url              = "https://github.com/parameshg/developer"
  }
}

# AWS LAMBDA ######################################################################################################################################################

resource "aws_lambda_function" "developer-api" {
  function_name  = "developer-api"
  role           = "${aws_iam_role.developer-api.arn}"
  package_type   = "Image"
  image_uri      = "${var.AWS_ACCOUNT}.dkr.ecr.${var.AWS_REGION}.amazonaws.com/developer-api:latest"
  image_config    {
    command      = var.IMAGE_COMMAND
  }
  environment      {
    variables    = {
      SENTRY_DSN = var.SENTRY_ENDPOINT
    }
  }
  timeout        = var.HTTP_TIMEOUT
  tags           = {
    provisioner  = "terraform"
    executioner  = "github-actions"
    project      = "developer-api"
    url          = "https://github.com/parameshg/developer"
  }
}

# AWS API GATEWAY #################################################################################################################################################

resource "aws_api_gateway_rest_api" "developer-api" {
  name                         = "developer-api"
  description                  = "Developer Api"
  disable_execute_api_endpoint = true
  endpoint_configuration {
    types                      = ["REGIONAL"]
  }
  tags                         = {
    provisioner                = "terraform"
    executioner                = "github-actions"
    project                    = "developer-api"
    url                        = "https://github.com/parameshg/developer"
  }
}

resource "aws_api_gateway_resource" "developer-api" {
  rest_api_id             = "${aws_api_gateway_rest_api.developer-api.id}"
  parent_id               = "${aws_api_gateway_rest_api.developer-api.root_resource_id}"
  path_part               = "{proxy+}"
}

resource "aws_api_gateway_method" "developer-api" {
  rest_api_id             = "${aws_api_gateway_rest_api.developer-api.id}"
  resource_id             = "${aws_api_gateway_resource.developer-api.id}"
  http_method             = "ANY"
  authorization           = "NONE"
  api_key_required        = true
}

resource "aws_api_gateway_method_response" "developer-api" {
    rest_api_id           = "${aws_api_gateway_rest_api.developer-api.id}"
    resource_id           = "${aws_api_gateway_resource.developer-api.id}"
    http_method           = "${aws_api_gateway_method.developer-api.http_method}"
    status_code           = "200"
    response_models       = {
      "application/json"  = "Empty"
    }
}

resource "aws_api_gateway_integration" "developer-api" {
  rest_api_id             = "${aws_api_gateway_rest_api.developer-api.id}"
  resource_id             = "${aws_api_gateway_method.developer-api.resource_id}"
  http_method             = "${aws_api_gateway_method.developer-api.http_method}"
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = "${aws_lambda_function.developer-api.invoke_arn}"
}

resource "aws_api_gateway_method" "developer-api-root" {
  rest_api_id             = "${aws_api_gateway_rest_api.developer-api.id}"
  resource_id             = "${aws_api_gateway_rest_api.developer-api.root_resource_id}"
  http_method             = "ANY"
  authorization           = "NONE"
  api_key_required        = true
}

resource "aws_api_gateway_integration" "developer-api-root" {
  rest_api_id             = "${aws_api_gateway_rest_api.developer-api.id}"
  resource_id             = "${aws_api_gateway_method.developer-api-root.resource_id}"
  http_method             = "${aws_api_gateway_method.developer-api-root.http_method}"
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = "${aws_lambda_function.developer-api.invoke_arn}"
}

resource "aws_api_gateway_deployment" "developer-api" {
  depends_on = [
    aws_api_gateway_integration.developer-api,
    aws_api_gateway_integration.developer-api-root,
  ]
  rest_api_id = "${aws_api_gateway_rest_api.developer-api.id}"
}

resource "aws_api_gateway_stage" "developer-api" {
  deployment_id = aws_api_gateway_deployment.developer-api.id
  rest_api_id   = aws_api_gateway_rest_api.developer-api.id
  stage_name    = "prod"
}

resource "aws_lambda_permission" "developer-api" {
  statement_id  = "AllowExecutionFromApiGateway"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.developer-api.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:${var.AWS_REGION}:${var.AWS_ACCOUNT}:${aws_api_gateway_rest_api.developer-api.id}/*"
}

# AWS API GATEWAY USAGE PLAN & API KEY ############################################################################################################################

resource "aws_api_gateway_usage_plan" "developer-api" {
  name              = "developer-api"
  description       = "Developer Api Usage Plan"
  api_stages {
    api_id          = aws_api_gateway_rest_api.developer-api.id
    stage           = aws_api_gateway_stage.developer-api.stage_name
  }
  quota_settings {
    limit           = 100
    offset          = 0
    period          = "DAY"
  }
  throttle_settings {
    burst_limit     = 2
    rate_limit      = 1
  }
}

resource "aws_api_gateway_api_key" "developer-api" {
  name = "developer-api"
}

resource "aws_api_gateway_usage_plan_key" "developer-api" {
  key_id        = aws_api_gateway_api_key.developer-api.id
  key_type      = "API_KEY"
  usage_plan_id = aws_api_gateway_usage_plan.developer-api.id
}

# AWS DYNAMODB ############################################################################################################################

resource "aws_dynamodb_table" "developer-projects" {
  name              = "projects"
  billing_mode      = "PAY_PER_REQUEST"
  hash_key          = "id"
  attribute {
    name            = "id"
    type            = "S"
  }
  global_secondary_index {
    name            = "id-index"
    hash_key        = "id"
    projection_type = "ALL"
  }
  tags              = {
    provisioner     = "terraform"
    executioner     = "github-actions"
    project         = "developer-api"
    url             = "https://github.com/parameshg/developer"
  }
}

resource "aws_dynamodb_table" "developer-licences" {
  name              = "licences"
  billing_mode      = "PAY_PER_REQUEST"
  hash_key          = "id"
  attribute {
    name            = "id"
    type            = "S"
  }
  global_secondary_index {
    name            = "id-index"
    hash_key        = "id"
    projection_type = "ALL"
  }
  tags              = {
    provisioner     = "terraform"
    executioner     = "github-actions"
    project         = "developer-api"
    url             = "https://github.com/parameshg/developer"
  }
}