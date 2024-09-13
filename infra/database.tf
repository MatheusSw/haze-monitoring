resource "aws_db_instance" "default" {
  allocated_storage           = 10
  db_name                     = local.db_name
  engine                      = local.db_engine
  instance_class              = "db.t3.micro"
  username                    = local.db_username
  manage_master_user_password = true
  skip_final_snapshot         = true
}
