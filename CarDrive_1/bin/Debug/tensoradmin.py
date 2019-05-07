import tensorflow as tf

def copy_tensors(dest_scope_name, src_scope_name):
    op_holder = []

    src_vars = tf.get_collection(tf.GraphKeys.TRAINABLE_VARIABLES, scope=src_scope_name)
    dest_vars = tf.get_collection(tf.GraphKeys.TRAINABLE_VARIABLES, scope=dest_scope_name)

    for src_var, dest_var in zip(src_vars, dest_vars):
        op_holder.append(dest_var.assign(src_var.value()))

    return op_holder

def save_tensors(filename, source_scope_name):
    f = open(filename, "w")
    src_vars = tf.get_collection(tf.GraphKeys.TRAINABLE_VARIABLES, scope=source_scope_name)
    for src_var in src_vars:
        #f.write(src_var)
        pass
